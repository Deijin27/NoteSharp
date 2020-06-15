using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Xamarin.Essentials;
using System.Runtime.InteropServices.ComTypes;

namespace Notes.Pages
{
    public class FolderContentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate_NameOnly { get; set; }
        public DataTemplate FolderTemplate_NameDateModified { get; set; }
        public DataTemplate FolderTemplate_NameDateCreated { get; set; }
        public DataTemplate FileTemplate_NameOnly { get; set; }
        public DataTemplate FileTemplate_NameDateModified { get; set; }
        public DataTemplate FileTemplate_NameDateCreated { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var contentItem = (FolderContentItem)item;

            SortingMode sortingMode = App.SortingMode;

            if (contentItem.Identifier == FolderContentItemIdentifier.Folder)
            {
                switch (sortingMode)
                {
                    case SortingMode.Name:
                        return FolderTemplate_NameOnly;
                    case SortingMode.DateCreated:
                        return FolderTemplate_NameDateCreated;
                    case SortingMode.DateModified:
                        return FolderTemplate_NameDateModified;
                    default:
                        return FolderTemplate_NameOnly;
                }
            }

            switch (sortingMode) // Note
            {
                case SortingMode.Name:
                    return FileTemplate_NameOnly;
                case SortingMode.DateCreated:
                    return FileTemplate_NameDateCreated;
                case SortingMode.DateModified:
                    return FileTemplate_NameDateModified;
                default:
                    return FileTemplate_NameOnly;
            }
        }
    }

    public partial class NotesPage : ContentPage
    {
        public int FolderID = 0;
        public bool IsQuickAccessPage = false;

        public NotesPage()
        {
            InitializeComponent();
            //InitializeListView();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateListView();
        }

        public async void InitializeListView()
        {
            await UpdateListView();
        }

        public async Task UpdateListView()
        {
            List<Folder> folderItems;
            List<Note> fileItems;

            if (IsQuickAccessPage)
            {
                folderItems = await App.Database.GetQuickAccessFoldersAsync();
                fileItems = await App.Database.GetQuickAccessNotesAsync();
            }
            else
            { 
                folderItems = await App.Database.GetFoldersAsync(FolderID);
                fileItems = await App.Database.GetNotesAsync(FolderID);
            }

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


        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NoteEntryPage(FolderID, this)));
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var folderContentItem = e.SelectedItem as FolderContentItem;

                if (folderContentItem.Identifier == FolderContentItemIdentifier.Folder)
                {
                    Folder folder = folderContentItem.ContentFolder;

                    await Navigation.PushAsync(new NotesPage
                    {
                        FolderID = folder.ID,
                        Title = folder.Name
                    });
                }
                else // Note
                {
                    await Navigation.PushModalAsync(new NavigationPage(new NoteEntryPage(folderContentItem.ContentNote, this)));
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

        private async void RenameFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            (Option option, string result) = await NameValidation.GetUniqueFolderName(this, folder.ParentID, "Rename Folder", 
                isQuickAccess: folder.IsQuickAccess,
                initialValue: folder.Name);
            if (option == Option.OK)
            {
                folder.Name = result;
                folder.DateModified = DateTime.UtcNow;
                await App.Database.SaveFolderAsync(folder);
                await UpdateListView();
            }
        }

        private async void MoveFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            await Navigation.PushModalAsync(new NavigationPage(new NotesMovePage(folder) { CurrentFolderName = this.Title }));
        }

        private async void DeleteFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            bool answer = await DisplayAlert("Delete Folder?", "Permanently delete folder and all contents?", "Yes", "No");
            if (answer)
            {
                await App.Database.DeleteFolderAndAllContentsAsync(folder);
                await UpdateListView();
            }
        }

        private async void RenameNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            (Option option, string result) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Rename Note", 
                isQuickAccess: note.IsQuickAccess,
                initialValue: note.Name);
            if (option == Option.OK)
            {
                note.Name = result;
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(note);
                await UpdateListView();
            }
        }

        private async void MoveNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            await Navigation.PushModalAsync(new NavigationPage(new NotesMovePage(note) { CurrentFolderName = this.Title }));
        }

        private async void DeleteNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            bool answer = await DisplayAlert("Delete Note?", "Are you sure you want to permanently delete this note?", "Yes", "No");
            if (answer)
            {
                await App.Database.DeleteNoteAsync(note);
                await UpdateListView();
            }
        }

        private async void ToggleNoteQuickAccess_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            if (!note.IsQuickAccess)
            {
                bool answer = await DisplayAlert("Quick Access", "Add note to quick access?", "Yes", "Cancel");

                if (answer)
                {
                    if (await App.Database.DoesQuickAccessNoteNameExistAsync(note.Name))
                    {
                        (Option option, string newName) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Note Name Conflict",
                            isQuickAccess: true,
                            message: "A note of the same name already exists in the QuickAccess, please input a different name");
                        if (option == Option.OK)
                        {
                            note.Name = newName;
                            note.IsQuickAccess = true;
                            await App.Database.SaveNoteAsync(note);
                            await UpdateListView();
                        }
                    }
                    else
                    {
                        note.IsQuickAccess = true;
                        await App.Database.SaveNoteAsync(note);
                        await UpdateListView();
                    }
                }
            }
            else
            {
                bool answer = await DisplayAlert("Quick Access", "Remove note from quick access?", "Yes", "Cancel");

                if (answer)
                {
                    note.IsQuickAccess = false;
                    await App.Database.SaveNoteAsync(note);
                    await UpdateListView();
                }
            }
        }

        

        private async void ToggleFolderQuickAccess_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            if (!folder.IsQuickAccess)
            {
                bool answer = await DisplayAlert("Quick Access", "Add folder to quick access?", "Yes", "Cancel");

                if (answer)
                {
                    if (await App.Database.DoesQuickAccessFolderNameExistAsync(folder.Name))
                    {
                        (Option option, string newName) = await NameValidation.GetUniqueFolderName(this, folder.ParentID, "Folder Name Conflict",
                            isQuickAccess: true,
                            message: "A folder of the same name already exists in the QuickAccess, please input a different name");
                        if (option == Option.OK)
                        {
                            folder.Name = newName;
                            folder.IsQuickAccess = true;
                            await App.Database.SaveFolderAsync(folder);
                            await UpdateListView();
                        }
                    }
                    else
                    {
                        folder.IsQuickAccess = true;
                        await App.Database.SaveFolderAsync(folder);
                        await UpdateListView();
                    }
                }
            }
            else
            {
                bool answer = await DisplayAlert("Quick Access", "Remove folder from quick access?", "Yes", "Cancel");

                if (answer)
                {
                    folder.IsQuickAccess = false;
                    await App.Database.SaveFolderAsync(folder);
                    await UpdateListView();
                }
            }
        }
    }
}