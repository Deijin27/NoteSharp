using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Xamarin.Essentials;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;

namespace Notes.Pages
{
    public enum MoveMode
    {
        Note,
        Folder
    }

    public class MoveCompletedEventArgs : EventArgs
    {
        public MoveCompletedEventArgs(Guid folderIDMovedTo, Guid folderIDMovedFrom)
        {
            FolderIDMovedTo = folderIDMovedTo;
            FolderIDMovedFrom = folderIDMovedFrom;
        }
        public readonly Guid FolderIDMovedTo;
        public readonly Guid FolderIDMovedFrom;
    }

    public delegate void MoveCompletedEventHandler(MoveCompletedEventArgs e);

    public partial class NotesMovePage : ContentPage
    {
        public Guid FolderID;
        public Folder FolderToMove;
        public Note NoteToMove;
        public MoveMode MoveMode;

        public string CurrentFolderName { set { Title = AppResources.PageTitle_Move + " " + value; } }

        public event MoveCompletedEventHandler MoveCompleted;

        public NotesMovePage(Folder folderToMove)
        {
            InitializeComponent();
            FolderID = folderToMove.ParentID;
            FolderToMove = folderToMove;
            MoveMode = MoveMode.Folder;
            UpdateListView();
        }
        public NotesMovePage(Note noteToMove)
        {
            InitializeComponent();
            FolderID = noteToMove.FolderID;
            NoteToMove = noteToMove;
            MoveMode = MoveMode.Note;
            UpdateListView();
        }

        public async void UpdateListView()
        {
            SortingMode sortingMode = App.SortingMode;

            var folderItems = (await App.Database.GetFoldersAsync(FolderID)).OrderBySortingMode(sortingMode).ToList();
            var fileItems = (await App.Database.GetNotesAsync(FolderID)).OrderBySortingMode(sortingMode).ToList();

            var listViewItems = new List<FolderContentItem>();

            foreach (Folder folder in folderItems)
            {
                listViewItems.Add(new FolderContentItem(folder));
            }
            foreach (Note file in fileItems)
            {
                listViewItems.Add(new FolderContentItem(file));
            }

            listView.ItemsSource = listViewItems;
        }
        
        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var folderContentItem = e.SelectedItem as FolderContentItem;

                // no need to check identifier as only folders can be selected
                Folder folder = folderContentItem.ContentFolder;
                CurrentFolderName = folder.Name;
                FolderID = folder.ID;
                UpdateListView();
                
                listView.SelectedItem = null;
            }
        }

        #region Add Folder

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            var popup = new UniqueNamePromptPopupPage
            (
                "New Folder",
                "Input name for folder",
                "Create",
                "Cancel",
                FolderID,
                Guid.Empty,
                App.Database.DoesFolderNameExistAsync,
                "Name invalid, it contains one of" + " \"/*.~",
                "Name conflict encountered; try a different name",
                "",
                "Input folder name..."
            );
            popup.AcceptOptionClicked += ProceedAddFolder;
            popup.CancelOptionClicked += CancelAddFolder;
            popup.BackgroundClicked += CancelAddFolder;
            popup.HardwareBackClicked += CancelAddFolder;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ProceedAddFolder(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            DateTime dateTime = DateTime.UtcNow;
            await App.Database.SaveAsync(new Folder
            {
                Name = e.Text,
                ParentID = FolderID,
                DateCreated = dateTime,
                DateModified = dateTime
            });
            UpdateListView();
        }

        private async void CancelAddFolder(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        private async void ToParentFolder_Clicked(object sender, EventArgs e)
        {
            
            if (FolderID != Guid.Empty)
            {
                Folder currentFolder = await App.Database.GetFolderAsync(FolderID);
                Guid parentID = currentFolder.ParentID;
                FolderID = parentID;
                UpdateListView();
                if (parentID != Guid.Empty)
                {
                    CurrentFolderName = (await App.Database.GetFolderAsync(parentID)).Name; 
                }
                else
                {
                    CurrentFolderName = "Root";
                }
            }
        }

        private async void SelectCurrentFolder_Clicked(object sender, EventArgs e)
        {
            if (MoveMode == MoveMode.Folder)
            {
                UniqueNamePromptPopupPage.DoesNameExist namechecker;
                if (FolderToMove.IsQuickAccess) namechecker = App.Database.DoesQuickAccessOrOtherwiseFolderNameExistAsync;
                else namechecker = App.Database.DoesFolderNameExistAsync;

                bool exists = await namechecker.Invoke(FolderToMove.Name, FolderID, FolderToMove.ID);
                if (exists)
                {
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Folder Name Conflict",
                        "Rename folder in order to move to the destination",
                        "OK",
                        "Cancel",
                        FolderID,
                        FolderToMove.ID,
                        namechecker,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        FolderToMove.Name,
                        "Input folder name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndMoveFolder;
                    popup.CancelOptionClicked += CancelRenameAndMoveFolder;
                    popup.BackgroundClicked += CancelRenameAndMoveFolder;
                    popup.HardwareBackClicked += CancelRenameAndMoveFolder;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    Guid sourceFolderID = FolderToMove.ParentID;
                    FolderToMove.ParentID = FolderID;
                    FolderToMove.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(FolderToMove);
                    MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                    await Navigation.PopModalAsync();
                }
            }
            else // i.e. MoveMode == MoveMode.Note
            {
                UniqueNamePromptPopupPage.DoesNameExist namechecker;
                if (NoteToMove.IsQuickAccess) namechecker = App.Database.DoesQuickAccessOrOtherwiseNoteNameExistAsync;
                else namechecker = App.Database.DoesNoteNameExistAsync;

                bool exists = await namechecker.Invoke(NoteToMove.Name, FolderID, NoteToMove.ID);
                if (exists)
                {
                    
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Note Name Conflict",
                        "Rename note in order to move to the destination",
                        "OK",
                        "Cancel",
                        FolderID,
                        NoteToMove.ID,
                        namechecker,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        NoteToMove.Name,
                        "Input folder name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndMoveNote;
                    popup.CancelOptionClicked += CancelRenameAndMoveNote;
                    popup.BackgroundClicked += CancelRenameAndMoveNote;
                    popup.HardwareBackClicked += CancelRenameAndMoveNote;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {

                    Guid sourceFolderID = NoteToMove.FolderID;
                    NoteToMove.FolderID = FolderID;
                    NoteToMove.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(NoteToMove);
                    MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                    await Navigation.PopModalAsync();
                }
            }
        }

        public async void ProceedRenameAndMoveFolder(PromptPopupOptionEventArgs e)
        {
            Guid sourceFolderID = FolderToMove.ParentID;
            FolderToMove.ParentID = FolderID;
            FolderToMove.DateModified = DateTime.UtcNow;
            FolderToMove.Name = e.Text;
            await App.Database.SaveAsync(FolderToMove);
            await PopupNavigation.Instance.PopAsync();
            MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
            await Navigation.PopModalAsync();
        }

        public async void CancelRenameAndMoveFolder(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async void CancelRenameAndMoveNote(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async void ProceedRenameAndMoveNote(PromptPopupOptionEventArgs e)
        {
            Guid sourceFolderID = NoteToMove.FolderID;
            NoteToMove.FolderID = FolderID;
            NoteToMove.DateModified = DateTime.UtcNow;
            NoteToMove.Name = e.Text;
            await App.Database.SaveAsync(NoteToMove);
            await PopupNavigation.Instance.PopAsync();
            MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
            await Navigation.PopModalAsync();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}