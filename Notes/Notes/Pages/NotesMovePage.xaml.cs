using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Xamarin.Essentials;

namespace Notes.Pages
{
    public enum MoveMode
    {
        Note,
        Folder
    }

    public partial class NotesMovePage : ContentPage
    {
        public int FolderID;
        public Folder FolderToMove;
        public Note NoteToMove;
        public MoveMode MoveMode;

        public string CurrentFolderName { set { Title = "Move To: " + value; } }

        public NotesMovePage(Folder folderToMove)
        {
            InitializeComponent();
            FolderID = folderToMove.ParentID;
            FolderToMove = folderToMove;
            MoveMode = MoveMode.Folder;
        }
        public NotesMovePage(Note noteToMove)
        {
            InitializeComponent();
            FolderID = noteToMove.FolderID;
            NoteToMove = noteToMove;
            MoveMode = MoveMode.Note;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateListView();
        }

        public async Task UpdateListView()
        {
            var folderItems = await App.Database.GetFoldersAsync(FolderID);
            var fileItems = await App.Database.GetNotesAsync(FolderID);

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
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var folderContentItem = e.SelectedItem as FolderContentItem;

                if (folderContentItem.Identifier == FolderContentItemIdentifier.Folder)
                {
                    Folder folder = folderContentItem.ContentFolder;
                    CurrentFolderName = folder.Name;
                    FolderID = folder.ID;
                    await UpdateListView();
                }
                else
                {
                    await DisplayAlert("What", "This shouldnt be happening", "OK");
                }
                listView.SelectedItem = null;
            }
        }

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            (Option option, string result) = await NameValidation.GetUniqueFolderName(this, FolderID, "New Folder");

            if (option == Option.OK)
            {
                DateTime dateTime = DateTime.UtcNow;
                await App.Database.CreateFolderAsync(new Folder 
                { 
                    Name = result, 
                    ParentID = FolderID, 
                    DateCreated = dateTime,
                    DateModified = dateTime
                });
                await UpdateListView();
            }

        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OrderBy_Clicked(object sender, EventArgs e)
        {
            string[] options =  { "Name", "Date Created", "Date Modified" };

            string selected = await DisplayActionSheet("Order By:", "Cancel", null, options);

            if (!string.IsNullOrEmpty(selected) && selected != "Cancel")
            {
                SortingMode sortingMode;

                switch (selected)
                {
                    case "Name":
                        sortingMode = SortingMode.Name;
                        break;
                    case "Date Created":
                        sortingMode = SortingMode.DateCreated;
                        break;
                    case "Date Modified":
                        sortingMode = SortingMode.DateModified;
                        break;
                    default:
                        sortingMode = SortingMode.Name;
                        break;
                }
                App.SortingMode = sortingMode;
                await UpdateListView();
            }
        }

        private async void ToParentFolder_Clicked(object sender, EventArgs e)
        {
            
            if (FolderID != 0)
            {
                Folder currentFolder = await App.Database.GetFolderAsync(FolderID);
                int parentID = currentFolder.ParentID;
                FolderID = parentID;
                await UpdateListView();
                if (parentID != 0)
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
                    (Option option, string newName) = await NameValidation.GetUniqueFolderName(this, FolderID, "Folder Name Conflict",
                        message: "A folder of the same name already exists in the destination, please input a different name");
                    if (option == Option.OK)
                    {
                        FolderToMove.ParentID = FolderID;
                        FolderToMove.DateModified = DateTime.UtcNow;
                        FolderToMove.Name = newName;
                        await App.Database.SaveFolderAsync(FolderToMove);
                        await Navigation.PopModalAsync();
                    }
                }
                else if (FolderToMove.IsQuickAccess && (await App.Database.DoesQuickAccessFolderNameExistAsync(FolderToMove.Name)))
                {
                    (Option option, string newName) = await NameValidation.GetUniqueFolderName(this, FolderID, "Folder Name Conflict",
                        isQuickAccess: true,
                        message: "A folder of the same name already exists in the QuickAccess, please input a different name");
                    if (option == Option.OK)
                    {
                        FolderToMove.ParentID = FolderID;
                        FolderToMove.DateModified = DateTime.UtcNow;
                        FolderToMove.Name = newName;
                        await App.Database.SaveFolderAsync(FolderToMove);
                        await Navigation.PopModalAsync();
                    }
                }
                else
                { 
                    FolderToMove.ParentID = FolderID;
                    FolderToMove.DateModified = DateTime.UtcNow;
                    await App.Database.SaveFolderAsync(FolderToMove);
                    await Navigation.PopModalAsync();
                }
            }
            else // i.e. MoveMode == MoveMode.Note
            {
                bool exists = await App.Database.DoesNoteNameExistAsync(NoteToMove.Name, FolderID);
                if (exists)
                {
                    (Option option, string newName) = await NameValidation.GetUniqueNoteName(this, FolderID, "Note Name Conflict",
                        message: "A note of the same name already exists in the destination, please input a different name");
                    if (option == Option.OK)
                    {
                        NoteToMove.FolderID = FolderID;
                        NoteToMove.DateModified = DateTime.UtcNow;
                        NoteToMove.Name = newName;
                        await App.Database.SaveNoteAsync(NoteToMove);
                        await Navigation.PopModalAsync();
                    }
                }
                else if (NoteToMove.IsQuickAccess && (await App.Database.DoesQuickAccessNoteNameExistAsync(NoteToMove.Name)))
                {
                    (Option option, string newName) = await NameValidation.GetUniqueNoteName(this, FolderID, "Note Name Conflict",
                        isQuickAccess: true,
                        message: "A note of the same name already exists in the QuickAccess, please input a different name");
                    if (option == Option.OK)
                    {
                        NoteToMove.FolderID = FolderID;
                        NoteToMove.DateModified = DateTime.UtcNow;
                        NoteToMove.Name = newName;
                        await App.Database.SaveNoteAsync(NoteToMove);
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    NoteToMove.FolderID = FolderID;
                    NoteToMove.DateModified = DateTime.UtcNow;
                    await App.Database.SaveNoteAsync(NoteToMove);
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