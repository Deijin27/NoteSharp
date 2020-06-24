using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Constants;
using Notes.Controls;

namespace Notes.Pages
{
    public class FolderContentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate_NameOnly { get; set; }
        public DataTemplate FolderTemplate_NameDateModified { get; set; }
        public DataTemplate FolderTemplate_NameDateCreated { get; set; }
        public DataTemplate FolderTemplate_NameSize { get; set; }
        public DataTemplate FileTemplate_NameOnly { get; set; }
        public DataTemplate FileTemplate_NameDateModified { get; set; }
        public DataTemplate FileTemplate_NameDateCreated { get; set; }
        public DataTemplate FileTemplate_NameSize { get; set; }

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
                    case SortingMode.Size:
                        return FolderTemplate_NameSize;
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
                case SortingMode.Size:
                    return FileTemplate_NameSize;
                default:
                    return FileTemplate_NameOnly;
            }
        }
    }

    public partial class NotesPage : ContentPage
    {
        public Guid FolderID;
        public bool IsQuickAccessPage = false;
        private List<FolderContentItem> FolderContentItems;
        string SearchFor = null;

        public NotesPage()
        {
            InitializeComponent();
            //InitializeListView();
            FolderID = Guid.Empty;
            UpdateListView();
        }

        public NotesPage(Folder folder)
        {
            InitializeComponent();
            Title = folder.Name;
            FolderID = folder.ID;
            UpdateListView();
        }

        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    UpdateListView();
        //}

        public void ChangesSavedHandler()
        {
            UpdateListView();
        }

        public async void UpdateListView()
        {
            await GetAllFoldersAndNotes();

            SearchUpdate();
        }

        public MoveCompletedEventHandler FolderContentMoved;

        
        public void MoveCompletedHandler(MoveCompletedEventArgs e)
        {
            if (FolderID == e.FolderIDMovedFrom)
            {
                UpdateListView();
            }
            if (FolderID == e.FolderIDMovedTo)
            {
                UpdateListView();
            }
            else // because if the folder moved to exists in the stack, it is for certian above the folder moved from.
            {
                FolderContentMoved?.Invoke(e);
            }
        }

        public void SearchUpdate()
        {
            if (string.IsNullOrEmpty(SearchFor))
            {
                listView.ItemsSource = FolderContentItems;
            }
            else 
            { 
                listView.ItemsSource = FolderContentItems.Where
                    (
                        i => (i.ContentFolder?.Name.Contains(SearchFor) ?? false)
                            || (i.ContentNote?.Name.Contains(SearchFor) ?? false)
                            || (i.ContentNote?.Text.Contains(SearchFor) ?? false)
                    ).ToList();
            }
        }

        public async Task GetAllFoldersAndNotes()
        {
            SortingMode sortingMode = App.SortingMode;

            FolderContentItems = new List<FolderContentItem>();

            List<Folder> contentFolders;
            List<Note> contentNotes;

            if (IsQuickAccessPage)
            {
                contentFolders = (await App.Database.GetQuickAccessFoldersAsync()).OrderBySortingMode(sortingMode).ToList();
                contentNotes = (await App.Database.GetQuickAccessNotesAsync()).OrderBySortingMode(sortingMode).ToList();
            }
            else
            {
                contentFolders = (await App.Database.GetFoldersAsync(FolderID)).OrderBySortingMode(sortingMode).ToList();
                contentNotes = (await App.Database.GetNotesAsync(FolderID)).OrderBySortingMode(sortingMode).ToList();
            }

            foreach (Folder folder in contentFolders)
            {
                FolderContentItems.Add(new FolderContentItem(folder));
            }
            foreach (Note file in contentNotes)
            {
                FolderContentItems.Add(new FolderContentItem(file));
            }
        }

        async void OnNoteAddedClicked(object sender, EventArgs e)
        {
            var page = new NoteEntryPage(FolderID, this);
            page.ChangesSaved += ChangesSavedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var folderContentItem = e.SelectedItem as FolderContentItem;

                if (folderContentItem.Identifier == FolderContentItemIdentifier.Folder)
                {
                    Folder folder = folderContentItem.ContentFolder;
                    var page = new NotesPage(folder);
                    page.FolderContentMoved += MoveCompletedHandler;
                    await Navigation.PushAsync(page);
                }
                else // Note
                {
                    var page = new NoteEntryPage(folderContentItem.ContentNote, this);
                    page.ChangesSaved += ChangesSavedHandler;
                    await Navigation.PushModalAsync(new NavigationPage(page));
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
            string selected = await DisplayActionSheet
            (
                "Order By:",
                ActionSheetOption.Cancel,
                null,
                ActionSheetOption.Name,
                ActionSheetOption.DateCreated,
                ActionSheetOption.DateModified,
                ActionSheetOption.Size
            );

            if (!string.IsNullOrEmpty(selected) && selected != ActionSheetOption.Cancel)
            {
                SortingMode sortingMode;

                switch (selected)
                {
                    case ActionSheetOption.Name:
                        sortingMode = SortingMode.Name;
                        break;
                    case ActionSheetOption.DateCreated:
                        sortingMode = SortingMode.DateCreated;
                        break;
                    case ActionSheetOption.DateModified:
                        sortingMode = SortingMode.DateModified;
                        break;
                    case ActionSheetOption.Size:
                        sortingMode = SortingMode.Size;
                        break;
                    default:
                        sortingMode = SortingMode.Name;
                        break;
                }
                App.SortingMode = sortingMode;
                UpdateListView();
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
                await App.Database.SaveAsync(folder);
                UpdateListView();
            }
        }

        private async void MoveFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;
            var page = new NotesMovePage(folder) { CurrentFolderName = this.Title };
            page.MoveCompleted += MoveCompletedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
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
                UpdateListView();
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
                await App.Database.SaveAsync(note);
                UpdateListView();
            }
        }

        private async void MoveNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;
            var page = new NotesMovePage(note) { CurrentFolderName = this.Title };
            page.MoveCompleted += MoveCompletedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        private async void DeleteNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            bool answer = await DisplayAlert("Delete Note?", "Are you sure you want to permanently delete this note?", "Yes", "No");
            if (answer)
            {
                await App.Database.DeleteAsync(note);
                UpdateListView();
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
                            await App.Database.SaveAsync(note);
                            UpdateListView();
                        }
                    }
                    else
                    {
                        note.IsQuickAccess = true;
                        await App.Database.SaveAsync(note);
                        UpdateListView();
                    }
                }
            }
            else
            {
                bool answer = await DisplayAlert("Quick Access", "Remove note from quick access?", "Yes", "Cancel");

                if (answer)
                {
                    note.IsQuickAccess = false;
                    await App.Database.SaveAsync(note);
                    UpdateListView();
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
                            await App.Database.SaveAsync(folder);
                            UpdateListView();
                        }
                    }
                    else
                    {
                        folder.IsQuickAccess = true;
                        await App.Database.SaveAsync(folder);
                        UpdateListView();
                    }
                }
            }
            else
            {
                bool answer = await DisplayAlert("Quick Access", "Remove folder from quick access?", "Yes", "Cancel");

                if (answer)
                {
                    folder.IsQuickAccess = false;
                    await App.Database.SaveAsync(folder);
                    UpdateListView();
                }
            }
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            var searchBar = new CustomSearchBar()
            {
                TextColor = Color.White,
                PlaceholderColor = Color.Gray,
                SearchIconColor = Color.White,
                CloseIconColor = Color.White,
                Placeholder = "Search Folder..."
            };
            searchBar.TextChanged += GetSearchResults;
            searchBar.CloseClicked += CloseSearchBar;

            NavigationPage.SetTitleView(this, searchBar);
            await Task.Delay(500); // necessary so that keyboard shows when searchBar.Focus is called
            searchBar.Focus();
            
        }

        private void GetSearchResults(object sender, TextChangedEventArgs e)
        {
            SearchFor = e.NewTextValue;
            SearchUpdate();
        }

        private void CloseSearchBar(object sender, EventArgs e)
        {
            var lb = new Label()
            {
                Text = Title,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20
            };

            NavigationPage.SetTitleView(this, lb);
            SearchFor = null;
            SearchUpdate();
        }
    }
}