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

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            (Option option, string result) = await NameValidation.GetUniqueFolderName
            (
                this, 
                FolderID, 
                AppResources.Prompt_NewFolder_Title
            );

            if (option == Option.OK)
            {
                DateTime dateTime = DateTime.UtcNow;
                await App.Database.SaveAsync(new Folder
                {
                    Name = result,
                    ParentID = FolderID,
                    DateCreated = dateTime,
                    DateModified = dateTime
                });
                UpdateListView();
            }
        }

        private async void OrderBy_Clicked(object sender, EventArgs e)
        {
            string option_cancel = AppResources.ActionSheetOption_Cancel;
            string option_name = AppResources.ActionSheetOption_OrderBy_Name;
            string option_dateCreated = AppResources.ActionSheetOption_OrderBy_DateCreated;
            string option_dateModified = AppResources.ActionSheetOption_OrderBy_DateModified;
            string option_size = AppResources.ActionSheetOption_OrderBy_Size;

            string selected = await DisplayActionSheet
            (
                AppResources.ActionSheetTitle_OrderBy,
                option_cancel,
                null,
                option_name,
                option_dateCreated,
                option_dateModified,
                option_size
            );

            if (!string.IsNullOrEmpty(selected) && selected != option_cancel)
            {
                SortingMode sortingMode;

                if (selected == option_name) sortingMode = SortingMode.Name;
                else if (selected == option_dateCreated) sortingMode = SortingMode.DateCreated;
                else if (selected == option_dateModified) sortingMode = SortingMode.DateModified;
                else if (selected == option_size) sortingMode = SortingMode.Size;
                else sortingMode = SortingMode.Name;

                App.SortingMode = sortingMode;
                UpdateListView();
            }
        }

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
                bool exists = await App.Database.DoesFolderNameExistAsync(FolderToMove.Name, FolderID);
                if (exists)
                {
                    (Option option, string newName) = await NameValidation.GetUniqueFolderName
                    (
                        this, 
                        FolderID, 
                        AppResources.Prompt_FolderNameConflict_Title,
                        message: AppResources.Prompt_FolderNameConflict_Message
                    );
                    if (option == Option.OK)
                    {
                        Guid sourceFolderID = FolderToMove.ParentID;
                        FolderToMove.ParentID = FolderID;
                        FolderToMove.DateModified = DateTime.UtcNow;
                        FolderToMove.Name = newName;
                        await App.Database.SaveAsync(FolderToMove);
                        MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                        await Navigation.PopModalAsync();
                    }
                }
                else if (FolderToMove.IsQuickAccess && (await App.Database.DoesQuickAccessFolderNameExistAsync(FolderToMove.Name)))
                {
                    (Option option, string newName) = await NameValidation.GetUniqueFolderName
                    (
                        this, 
                        FolderID,
                        AppResources.Prompt_FolderNameConflict_Title,
                        isQuickAccess: true,
                        message: AppResources.Prompt_QuickAccessFolderNameConflict_Message
                    );
                    if (option == Option.OK)
                    {
                        Guid sourceFolderID = FolderToMove.ParentID;
                        FolderToMove.ParentID = FolderID;
                        FolderToMove.DateModified = DateTime.UtcNow;
                        FolderToMove.Name = newName;
                        await App.Database.SaveAsync(FolderToMove);
                        MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                        await Navigation.PopModalAsync();
                    }
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
                bool exists = await App.Database.DoesNoteNameExistAsync(NoteToMove.Name, FolderID);
                if (exists)
                {
                    (Option option, string newName) = await NameValidation.GetUniqueNoteName
                    (
                        this, 
                        FolderID, 
                        AppResources.Prompt_NoteNameConflict_Title,
                        message: AppResources.Prompt_NoteNameConflict_Message
                    );
                    if (option == Option.OK)
                    {
                        Guid sourceFolderID = NoteToMove.FolderID;
                        NoteToMove.FolderID = FolderID;
                        NoteToMove.DateModified = DateTime.UtcNow;
                        NoteToMove.Name = newName;
                        await App.Database.SaveAsync(NoteToMove);
                        MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                        await Navigation.PopModalAsync();
                    }
                }
                else if (NoteToMove.IsQuickAccess && (await App.Database.DoesQuickAccessNoteNameExistAsync(NoteToMove.Name)))
                {
                    (Option option, string newName) = await NameValidation.GetUniqueNoteName
                    (
                        this, 
                        FolderID, 
                        AppResources.Prompt_NoteNameConflict_Title,
                        isQuickAccess: true,
                        message: AppResources.Prompt_QuickAccessNoteNameConflict_Message
                    );
                    if (option == Option.OK)
                    {
                        Guid sourceFolderID = NoteToMove.FolderID;
                        NoteToMove.FolderID = FolderID;
                        NoteToMove.DateModified = DateTime.UtcNow;
                        NoteToMove.Name = newName;
                        await App.Database.SaveAsync(NoteToMove);
                        MoveCompleted?.Invoke(new MoveCompletedEventArgs(FolderID, sourceFolderID));
                        await Navigation.PopModalAsync();
                    }
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

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}