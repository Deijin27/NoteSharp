using Notes.Models;
using Notes.RouteUtil;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public enum MoveMode
    {
        Note,
        Folder
    }

    public class MovePageViewModel : FolderViewModelBase, IQueryableViewModel<MovePageSetupParameters>
    {
        public MovePageViewModel(IAppServiceProvider services) : base(services)
        {
            ToParentFolderCommand = new Command(ToParentFolder);
            MoveToCurrentFolderCommand = new Command(MoveToCurrentFolder);
        }

        public override void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var p = new MovePageSetupParameters();
            p.FromQuery(query);
            Setup(p);
        }

        public async void Setup(MovePageSetupParameters setupParameters)
        {
            MoveMode = setupParameters.Mode;
            ItemToMove = setupParameters.ItemToMoveId;
            if (setupParameters.CurrentFolderId == Guid.Empty)
            {
                CurrentFolder = new Folder() { Name = "Root", ID = Guid.Empty };
            }
            else
            {
                CurrentFolder = await Services.NoteDatabase.GetFolderAsync(setupParameters.CurrentFolderId);
            }
            UpdateListView();
        }

        protected override void OnListViewItemSelected(FolderContentItem item)
        {
            if (item.Identifier == FolderContentItemIdentifier.Folder)
            {
                CurrentFolder = item.ContentFolder;
                UpdateListView();
            }
        }

        private MoveMode MoveMode;
        private Guid ItemToMove;
        private Folder _currentFolder;
        private Folder CurrentFolder
        {
            get => _currentFolder;
            set
            {
                _currentFolder = value;
                CurrentFolderId = value.ID;
                PageTitle = $"Move To: {value.Name}";
                RaisePropertyChanged(nameof(IsRootFolder));
            }
        }

        public bool IsRootFolder => CurrentFolderId == Guid.Empty;

        public ICommand MoveToCurrentFolderCommand { get; set; }
        public ICommand ToParentFolderCommand { get; set; }

        public async void MoveToCurrentFolder()
        {
            switch (MoveMode)
            {
                case MoveMode.Folder:
                    var existingFolder = await Services.NoteDatabase.GetFolderAsync(ItemToMove);
                    existingFolder.ParentID = CurrentFolderId;
                    existingFolder.DateModified = DateTime.UtcNow;
                    await Services.NoteDatabase.SaveAsync(existingFolder);
                    break;
                case MoveMode.Note:
                    var existingNote = await Services.NoteDatabase.GetNoteAsync(ItemToMove);
                    existingNote.FolderID = CurrentFolderId;
                    existingNote.DateModified = DateTime.UtcNow;
                    await Services.NoteDatabase.SaveAsync(existingNote);
                    break;
                default:
                    throw new Exception($"Invalid {nameof(MoveMode)}");
            }
            await Services.Navigation.GoBackAsync();
        }

        private async void ToParentFolder()
        {
            if (IsRootFolder)
            {
                return;
            }

            if (CurrentFolder.ParentID == Guid.Empty)
            {
                CurrentFolder = new Folder() { Name = "Root", ID = Guid.Empty };
            }
            else
            {
                CurrentFolder = await Services.NoteDatabase.GetFolderAsync(CurrentFolder.ParentID);
            }
            
            UpdateListView();
        }
    }
}
