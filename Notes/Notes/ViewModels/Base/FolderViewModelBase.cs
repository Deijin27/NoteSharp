using Notes.Models;
using Notes.Resources;
using Notes.RouteUtil;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public abstract class FolderViewModelBase : ViewModelBase, IQueryAttributable
    {
        protected readonly IAppServiceProvider Services;
        public FolderViewModelBase(IAppServiceProvider services)
        {
            Services = services;

            RenameFolderCommand = new Command(RenameFolder);
            MoveFolderCommand = new Command(MoveFolder);
            DeleteFolderCommand = new Command(DeleteFolder);

            RenameNoteCommand = new Command(RenameNote);
            MoveNoteCommand = new Command(MoveNote);
            DeleteNoteCommand = new Command(DeleteNote);

            AddNoteCommand = new Command(AddNote);
            AddFolderCommand = new Command(AddFolder);
            OrderByCommand = new Command(OrderBy);

            ToggleQuickAccessFolderCommand = new Command(ToggleQuickAccessFolder);
            ToggleQuickAccessNoteCommand = new Command(ToggleQuickAccessNote);
        }


        public ICommand MoveFolderCommand { get; set; }
        public ICommand MoveNoteCommand { get; set; }
        public ICommand AddNoteCommand { get; set; }
        public ICommand OrderByCommand { get; set; }
        public ICommand AddFolderCommand { get; set; }
        public ICommand RenameFolderCommand { get; set; }
        public ICommand DeleteFolderCommand { get; set; }
        public ICommand RenameNoteCommand { get; set; }
        public ICommand DeleteNoteCommand { get; set; }
        public ICommand ToggleQuickAccessFolderCommand { get; set; }
        public ICommand ToggleQuickAccessNoteCommand { get; set; }


        protected Guid CurrentFolderId;

        private List<FolderContentItem> _folderContentItems;
        public List<FolderContentItem> FolderContentItems
        {
            get => _folderContentItems;
            set => RaiseAndSetIfChanged(ref _folderContentItems, value);
        }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => RaiseAndSetIfChanged(ref _pageTitle, value);
        }

        private FolderContentItem _listViewSelectedItem;
        public FolderContentItem ListViewSelectedItem
        {
            get => _listViewSelectedItem;
            set
            {
                if (value != _listViewSelectedItem)
                {
                    _listViewSelectedItem = value;
                    OnListViewItemSelected(value);
                }
            }
        }

        protected async virtual void OnListViewItemSelected(FolderContentItem item)
        {
            switch (item.Identifier)
            {
                case FolderContentItemIdentifier.Folder:
                    await Services.Navigation.GoToAsync<FolderPageViewModel, FolderPageSetupParameters>(
                        new FolderPageSetupParameters() { FolderId = item.ContentFolder.ID });
                    break;
                case FolderContentItemIdentifier.File:
                    await Services.Navigation.GoToAsync<NotePageViewModel, NotePageSetupParameters>(
                        NotePageSetupParameters.ExistingNote(item.ContentNote.ID));
                    break;
                default:
                    throw new Exception($"Invalid {nameof(FolderContentItemIdentifier)}");
            }
        }

        protected async virtual void UpdateListView()
        {
            SortingMode sortingMode = Services.Preferences.SortingMode;

            var items = new List<FolderContentItem>();

            var folders = await Services.NoteDatabase.GetFoldersAsync(CurrentFolderId);
            var notes = await Services.NoteDatabase.GetNotesAsync(CurrentFolderId);

            IEnumerable<FolderContentItem> contentFolders = folders
                .OrderBySortingMode(sortingMode)
                .Select(i => new FolderContentItem(i));
            IEnumerable<FolderContentItem> contentNotes = notes
                .OrderBySortingMode(sortingMode)
                .Select(i => new FolderContentItem(i));

            //items.Capacity = folders.Count + notes.Count;
            items.AddRange(contentFolders);
            items.AddRange(contentNotes);
            FolderContentItems = items;
        }

        async void AddNote()
        {
            await Services.Navigation.GoToAsync<NotePageViewModel, NotePageSetupParameters>(
            NotePageSetupParameters.NewNote(CurrentFolderId));
        }

        private async void OrderBy()
        {
            var result = await Services.Popups.ListPopup
            (
                "Order By",
                "Pick an sorting method",
                AppResources.ActionSheetOption_Cancel,
                new List<ListPopupPageItem<SortingMode>>
                {
                    new ListPopupPageItem<SortingMode> { Name = AppResources.ActionSheetOption_OrderBy_Name, AssociatedObject = SortingMode.Name },
                    new ListPopupPageItem<SortingMode> { Name = AppResources.ActionSheetOption_OrderBy_DateCreated, AssociatedObject = SortingMode.DateCreated },
                    new ListPopupPageItem<SortingMode> { Name = AppResources.ActionSheetOption_OrderBy_DateModified, AssociatedObject = SortingMode.DateModified },
                    new ListPopupPageItem<SortingMode> { Name = AppResources.ActionSheetOption_OrderBy_Size, AssociatedObject = SortingMode.Size }
                }
            );
            if (result.Choice == ListPopupResult.ListItem)
            {
                Services.Preferences.SortingMode = result.SelectedItem;
                UpdateListView();
            }
        }

        protected async void AddFolder()
        {
            var result = await Services.Popups.PromptPopup
            (
                title: "New Folder",
                message: "Input name for folder",
                leftButtonText: "Create",
                rightButtonText: "Cancel",
                placeholderText: "Input folder name..."
            );

            if (result.Selected == TwoOptionPopupResult.LeftButton)
            {
                DateTime dateTime = DateTime.UtcNow;
                await Services.NoteDatabase.SaveAsync(new Folder
                {
                    Name = result.Text,
                    ParentID = CurrentFolderId,
                    DateCreated = dateTime,
                    DateModified = dateTime
                });
                UpdateListView();
            }
        }

        private async void RenameFolder(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            Folder folder = folderContentItem.ContentFolder;

            var result = await Services.Popups.PromptPopup
            (
                title: "Rename Folder",
                message: "Input new name for folder",
                leftButtonText: "OK",
                rightButtonText: "Cancel",
                initialEntryText: folder.Name,
                placeholderText: "Input folder name..."
            );

            if (result.Selected == TwoOptionPopupResult.LeftButton)
            {
                folder.Name = result.Text;
                folder.DateModified = DateTime.UtcNow;
                await Services.NoteDatabase.SaveAsync(folder);
                UpdateListView();
            }
        }

        private void MoveFolder(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            Folder folder = folderContentItem.ContentFolder;
            Services.Navigation.GoToAsync<MovePageViewModel, MovePageSetupParameters>(new MovePageSetupParameters(folder));
        }

        private async void DeleteFolder(object commandParameter)
        {
            FolderContentItem folderContentItem = commandParameter as FolderContentItem;
            var folder = folderContentItem.ContentFolder;

            var result = await Services.Popups.TwoOptionPopup
            (
                AppResources.Alert_ConfirmDeleteFolder_Title,
                AppResources.Alert_ConfirmDeleteFolder_Message,
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );

            if (result == TwoOptionPopupResult.LeftButton)
            {
                await Services.NoteDatabase.DeleteFolderAndAllContentsAsync(folder);
                //UpdateListView();
                FolderContentItems.Remove(folderContentItem);
                RaisePropertyChanged(nameof(FolderContentItems));
            }
        }

        private async void RenameNote(object commandParameter)
        {
            FolderContentItem folderContentItem = commandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            var result = await Services.Popups.PromptPopup
            (
                title: "Rename Note",
                message: "Input new name for note",
                leftButtonText: "OK",
                rightButtonText: "Cancel",
                initialEntryText: note.Name,
                placeholderText: "Input note name..."
            );

            if (result.Selected == TwoOptionPopupResult.LeftButton)
            {
                note.Name = result.Text;
                note.DateModified = DateTime.UtcNow;
                await Services.NoteDatabase.SaveAsync(note);
                UpdateListView();
            }
        }

        private void MoveNote(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            Note note = folderContentItem.ContentNote;
            Services.Navigation.GoToAsync<MovePageViewModel, MovePageSetupParameters>(new MovePageSetupParameters(note));
        }

        private async void DeleteNote(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            var note = folderContentItem.ContentNote;

            var result = await Services.Popups.TwoOptionPopup
            (
                AppResources.Alert_ConfirmDeleteNote_Title,
                AppResources.Alert_ConfirmDeleteNote_Message,
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );

            if (result == TwoOptionPopupResult.LeftButton)
            {
                await Services.NoteDatabase.DeleteAsync(note);
                //UpdateListView();
                FolderContentItems.Remove(folderContentItem);
                RaisePropertyChanged(nameof(FolderContentItems));
            }
        }

        public abstract void ApplyQueryAttributes(IDictionary<string, string> query);

        private async void ToggleQuickAccessFolder(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            var folder = folderContentItem.ContentFolder;
            folder.IsQuickAccess = !folder.IsQuickAccess;
            folder.DateModified = DateTime.UtcNow;
            await Services.NoteDatabase.SaveAsync(folder);
            UpdateListView();
        }

        private async void ToggleQuickAccessNote(object commandParameter)
        {
            FolderContentItem folderContentItem = (FolderContentItem)commandParameter;
            var note = folderContentItem.ContentNote;
            note.IsQuickAccess = !note.IsQuickAccess;
            note.DateModified = DateTime.UtcNow;
            await Services.NoteDatabase.SaveAsync(note);
            UpdateListView();
        }
    }
}
