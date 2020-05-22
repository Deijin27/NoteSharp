using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;

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

            SortingMode sortingMode = App.GetSortingMode();

            if (contentItem.Identifier == "Folder")
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

            switch (sortingMode)
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
        public int FolderID;

        public NotesPage()
        {
            InitializeComponent();
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


        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new Note(),
                NewNote = true,
                FolderID = FolderID
            });
        }
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var folderContentItem = e.SelectedItem as FolderContentItem;

                if (folderContentItem.Identifier == "Folder")
                {
                    Folder folder = folderContentItem.ContentFolder;

                    await Navigation.PushAsync(new NotesPage
                    {
                        FolderID = folder.ID,
                        Title = folder.Name
                    });
                }
                else
                {
                    await Navigation.PushAsync(new NoteEntryPage
                    {
                        BindingContext = folderContentItem.ContentNote,
                        FolderID = FolderID
                    });
                }
                listView.SelectedItem = null;
            }
        }

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Create Folder", "Input folder name", "Create");

            DateTime dateTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(result))
            {
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

            Console.WriteLine($"--+-+-+-+-+-+ Action [{selected}] -+-++-+-+");
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
                App.SetSortingMode(sortingMode);
                await UpdateListView();
            }
        }

        private async void RenameFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            string result  = await DisplayPromptAsync("Rename Folder", "What should the folder be renamed to?", initialValue: folder.Name);
            if (!string.IsNullOrWhiteSpace(result))
            {
                folder.Name = result;
                folder.DateModified = DateTime.UtcNow;
                await App.Database.SaveFolderAsync(folder);
                await UpdateListView();
            }
        }

        private async void MoveFolder_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("MoveFolder_Clicked", "The other one", "OK");
        }

        private async void DeleteFolder_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("DeleteFolder_Clicked", "Delete Folder Invoked", "OK");
        }

        private async void RenameNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            string result = await DisplayPromptAsync("Rename Note", "What should the folder be renamed to?", initialValue: note.Name);
            if (!string.IsNullOrWhiteSpace(result))
            {
                note.Name = result;
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(note);
                await UpdateListView();
            }
        }

        private async void MoveNote_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("MoveNote_Clicked", "The other one", "OK");
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
    }
}