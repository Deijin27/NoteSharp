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
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate FileTemplate_NameOnly { get; set; }
        public DataTemplate FileTemplate_NameDateModified { get; set; }
        public DataTemplate FileTemplate_NameDateCreated { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var contentItem = (FolderContentItem)item;
            if (contentItem.Identifier == "Folder")
                return FolderTemplate;

            SortingMode sortingMode = App.GetSortingMode();

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
            }
        }

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Create Folder", "Input folder name", "Create");

            if (result != null)
            {
                await App.Database.CreateFolderAsync(new Folder { Name = result, ParentID = FolderID });
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
            
            if (selected != "Cancel")
            {
                SortingMode sortingMode;

                switch (selected)
                {
                    case "Name":
                        sortingMode = SortingMode.Name;
                        break;
                    case "DateCreated":
                        sortingMode = SortingMode.DateCreated;
                        break;
                    case "DateModified":
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
    }
}