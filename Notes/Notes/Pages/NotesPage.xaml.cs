using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Controls;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;
using Notes.Views;

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

    public delegate void SortingModeChangedEventHandler();

    public partial class NotesPage : ContentPage
    {
        public Guid FolderID;
        public bool IsQuickAccessPage = false;
        private List<FolderContentItem> FolderContentItems;
        string SearchFor = null;

        public NotesPage(bool isQuickAccessPage = false)
        {
            IsQuickAccessPage = isQuickAccessPage;
            InitializeComponent();
            //InitializeListView();
            FolderID = Guid.Empty;
            UpdateListView();

            if (isQuickAccessPage)
            {
                ToolbarItems.Remove(AddNoteButton);
                ToolbarItems.Remove(AddFolderButton);
            }
        }

        public NotesPage(Folder folder)
        {
            InitializeComponent();
            Title = folder.Name;
            FolderID = folder.ID;
            UpdateListView();
        }

        public void ChangesSavedHandler()
        {
            UpdateListView();
        }

        public void SortingModeChangedHandler()
        {
            UpdateListView();
            SortingModeChanged?.Invoke();
        }

        public async void UpdateListView()
        {
            await GetAllFoldersAndNotes();

            SearchUpdate();
        }

        public event MoveCompletedEventHandler FolderContentMoved;
        
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
            var page = new NoteEntryPage(FolderID);
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
                    page.SortingModeChanged += SortingModeChangedHandler;
                    await Navigation.PushAsync(page);
                }
                else // Note
                {
                    var page = new NoteEntryPage(folderContentItem.ContentNote);
                    page.ChangesSaved += ChangesSavedHandler;
                    await Navigation.PushModalAsync(new NavigationPage(page));
                }
                listView.SelectedItem = null;
            }
        }

        #region Add Folder

        async void OnFolderAddedClicked(object sender, EventArgs e)
        {
            #region old stuff
            /*
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
            */
            #endregion

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

        #region Order By
        private async void OrderBy_Clicked(object sender, EventArgs e)
        {
            var popup = new ListPopupPage
                (
                    "Order By",
                    "Pick an sorting method",
                    AppResources.ActionSheetOption_Cancel,
                    new List<ListPopupPageItem>
                    {
                        new ListPopupPageItem { Name = AppResources.ActionSheetOption_OrderBy_Name, AssociatedObject = SortingMode.Name },
                        new ListPopupPageItem { Name = AppResources.ActionSheetOption_OrderBy_DateCreated, AssociatedObject = SortingMode.DateCreated },
                        new ListPopupPageItem { Name = AppResources.ActionSheetOption_OrderBy_DateModified, AssociatedObject = SortingMode.DateModified },
                        new ListPopupPageItem { Name = AppResources.ActionSheetOption_OrderBy_Size, AssociatedObject = SortingMode.Size }
                    }
                );
            popup.CancelClicked += CancelOrderBy;
            popup.BackgroundClicked += CancelOrderBy;
            popup.HardwareBackClicked += CancelOrderBy;
            popup.ListOptionClicked += ProceedOrderBy;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        async void ProceedOrderBy(ListPopupPageItem selected)
        {
            await PopupNavigation.Instance.PopAsync();
            App.SortingMode = (SortingMode)selected.AssociatedObject;
            UpdateListView();
            SortingModeChanged?.Invoke();
        }

        async void CancelOrderBy()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public event SortingModeChangedEventHandler SortingModeChanged;

        #endregion

        #region Rename Folder

        private async void RenameFolder_Clicked(object sender, EventArgs e)
        {
            #region Old Stuff
            /*
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            (Option option, string result) = await NameValidation.GetUniqueFolderName
            (
                this, 
                folder.ParentID, 
                AppResources.Prompt_RenameFolder_Title, 
                isQuickAccess: folder.IsQuickAccess,
                initialValue: folder.Name
            );

            if (option == Option.OK)
            {
                folder.Name = result;
                folder.DateModified = DateTime.UtcNow;
                await App.Database.SaveAsync(folder);
                UpdateListView();
            }*/
            #endregion


            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;
            FolderBeingProcessed = folder;

            UniqueNamePromptPopupPage.DoesNameExist doesNameExistChecker;
            if (folder.IsQuickAccess)
                doesNameExistChecker = App.Database.DoesQuickAccessOrOtherwiseFolderNameExistAsync;
            else
                doesNameExistChecker = App.Database.DoesFolderNameExistAsync;

            var popup = new UniqueNamePromptPopupPage
            (
                "Rename Folder",
                "Input new name for folder",
                "OK",
                "Cancel",
                folder.ParentID,
                folder.ID,
                doesNameExistChecker,
                "Name invalid, it contains one of" +  " \"/*.~",
                "Name conflict encountered; try a different name",
                folder.Name,
                "Input folder name..."
            );
            popup.AcceptOptionClicked += ProceedRenameFolder;
            popup.CancelOptionClicked += CancelRenameFolder;
            popup.BackgroundClicked += CancelRenameFolder;
            popup.HardwareBackClicked += CancelRenameFolder;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private Folder FolderBeingProcessed;

        private async void ProceedRenameFolder(PromptPopupOptionEventArgs e)
        {

            await PopupNavigation.Instance.PopAsync();
            FolderBeingProcessed.Name = e.Text;
            FolderBeingProcessed.DateModified = DateTime.UtcNow;
            await App.Database.SaveAsync(FolderBeingProcessed);
            FolderBeingProcessed = null;
            UpdateListView();

        }

        private async void CancelRenameFolder(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            FolderBeingProcessed = null;
        }

        #endregion

        private async void MoveFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;
            var page = new NotesMovePage(folder) { CurrentFolderName = this.Title };
            page.MoveCompleted += MoveCompletedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        #region Delete Folder

        private async void DeleteFolder_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            FolderBeingProcessed = folderContentItem.ContentFolder;

            var popup = new TwoOptionPopupPage
            (
                AppResources.Alert_ConfirmDeleteFolder_Title,
                AppResources.Alert_ConfirmDeleteFolder_Message,
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );
            popup.LeftOptionClicked += ProceedDeleteFolder;
            popup.RightOptionClicked += CancelDeleteFolder;
            popup.BackgroundClicked += CancelDeleteFolder;
            popup.HardwareBackClicked += CancelDeleteFolder;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ProceedDeleteFolder()
        {

            await PopupNavigation.Instance.PopAsync();
            await App.Database.DeleteFolderAndAllContentsAsync(FolderBeingProcessed);
            FolderBeingProcessed = null;
            UpdateListView();

        }

        private async void CancelDeleteFolder()
        {
            await PopupNavigation.Instance.PopAsync();
            FolderBeingProcessed = null;
        }

        #endregion

        #region Rename Note

        private async void RenameNote_Clicked(object sender, EventArgs e)
        {
            #region old stuff
            /*
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            (Option option, string result) = await NameValidation.GetUniqueNoteName
            (
                this, 
                note.FolderID, 
                AppResources.Prompt_RenameNote_Title, 
                isQuickAccess: note.IsQuickAccess,
                initialValue: note.Name
            );

            if (option == Option.OK)
            {
                note.Name = result;
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveAsync(note);
                UpdateListView();
            }
            */
            #endregion

            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;
            NoteBeingProcessed = note;

            UniqueNamePromptPopupPage.DoesNameExist doesNameExistChecker;
            if (note.IsQuickAccess) 
                doesNameExistChecker = App.Database.DoesQuickAccessOrOtherwiseNoteNameExistAsync;
            else
                doesNameExistChecker = App.Database.DoesNoteNameExistAsync;

            var popup = new UniqueNamePromptPopupPage
            (
                "Rename Note",
                "Input new name for note",
                "OK",
                "Cancel",
                note.FolderID,
                note.ID,
                doesNameExistChecker,
                "Name invalid, it contains one of" + " \"/*.~",
                "Name conflict encountered; try a different name",
                note.Name,
                "Input note name..."
            );
            popup.AcceptOptionClicked += ProceedRenameNote;
            popup.CancelOptionClicked += CancelRenameNote;
            popup.BackgroundClicked += CancelRenameNote;
            popup.HardwareBackClicked += CancelRenameNote;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private Note NoteBeingProcessed;

        private async void ProceedRenameNote(PromptPopupOptionEventArgs e)
        {
            
            await PopupNavigation.Instance.PopAsync();
            NoteBeingProcessed.Name = e.Text;
            NoteBeingProcessed.DateModified = DateTime.UtcNow;
            await App.Database.SaveAsync(NoteBeingProcessed);
            NoteBeingProcessed = null;
            UpdateListView();
            
        }

        private async void CancelRenameNote(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            NoteBeingProcessed = null;
        }

        #endregion

        private async void MoveNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;
            var page = new NotesMovePage(note) { CurrentFolderName = this.Title };
            page.MoveCompleted += MoveCompletedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }

        #region Delete Note

        private async void DeleteNote_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            NoteBeingProcessed = folderContentItem.ContentNote;

            var popup = new TwoOptionPopupPage
            (
                AppResources.Alert_ConfirmDeleteNote_Title,
                AppResources.Alert_ConfirmDeleteNote_Message,
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );
            popup.LeftOptionClicked += ProceedDeleteNote;
            popup.RightOptionClicked += CancelDeleteNote;
            popup.BackgroundClicked += CancelDeleteNote;
            popup.HardwareBackClicked += CancelDeleteNote;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ProceedDeleteNote()
        {

            await PopupNavigation.Instance.PopAsync();
            await App.Database.DeleteAsync(NoteBeingProcessed);
            NoteBeingProcessed = null;
            UpdateListView();

        }

        private async void CancelDeleteNote()
        {
            await PopupNavigation.Instance.PopAsync();
            NoteBeingProcessed = null;
        }

        #endregion

        #region Toggle Note Quick Access
        private async void ToggleNoteQuickAccess_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Note note = folderContentItem.ContentNote;

            #region Old Stuff
            /*

            if (!note.IsQuickAccess)
            {
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_AddNoteToQuickAccess_Title, 
                    AppResources.Alert_AddNoteToQuickAccess_Message, 
                    AppResources.AlertOption_Yes, 
                    AppResources.AlertOption_Cancel
                );

                if (answer)
                {
                    if (await App.Database.DoesQuickAccessNoteNameExistAsync(note.Name))
                    {
                        (Option option, string newName) = await NameValidation.GetUniqueNoteName
                        (
                            this, 
                            note.FolderID, 
                            AppResources.Prompt_NoteNameConflict_Title,
                            isQuickAccess: true,
                            message: AppResources.Prompt_QuickAccessNoteNameConflict_Message
                        );

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
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_RemoveNoteFromQuickAccess_Title, 
                    AppResources.Alert_RemoveNoteFromQuickAccess_Message, 
                    AppResources.AlertOption_Yes, 
                    AppResources.AlertOption_Cancel
                );

                if (answer)
                {
                    note.IsQuickAccess = false;
                    await App.Database.SaveAsync(note);
                    UpdateListView();
                }
            }
            */
            #endregion

            if (!note.IsQuickAccess)
            {
                if (await App.Database.DoesQuickAccessNoteNameExistAsync(note.Name, note.ID))
                {
                    NoteBeingProcessed = note;
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Name Conflict Encountered",
                        "In order to add the note to quick access, it must be renamed",
                        "OK",
                        "Cancel",
                        note.FolderID,
                        note.ID,
                        App.Database.DoesQuickAccessOrOtherwiseNoteNameExistAsync,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        note.Name,
                        "Input note name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameNoteThenAddToQuickAccess;
                    popup.CancelOptionClicked += CancelAddNoteToQuickAccess;
                    popup.BackgroundClicked += CancelAddNoteToQuickAccess;
                    popup.HardwareBackClicked += CancelAddNoteToQuickAccess;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    note.IsQuickAccess = true;
                    note.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(note);
                    UpdateListView();
                }
            }
            else
            {
                note.IsQuickAccess = false;
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveAsync(note);
                UpdateListView();
            }
        }

        private async void ProceedRenameNoteThenAddToQuickAccess(PromptPopupOptionEventArgs e)
        {

            await PopupNavigation.Instance.PopAsync();
            NoteBeingProcessed.Name = e.Text;
            NoteBeingProcessed.IsQuickAccess = true;
            NoteBeingProcessed.DateModified = DateTime.UtcNow;
            await App.Database.SaveAsync(NoteBeingProcessed);
            NoteBeingProcessed = null;
            UpdateListView();

        }

        private async void CancelAddNoteToQuickAccess(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            NoteBeingProcessed = null;
        }
        #endregion

        #region Toggle Folder Quick Access
        private async void ToggleFolderQuickAccess_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            FolderContentItem folderContentItem = mi.CommandParameter as FolderContentItem;
            Folder folder = folderContentItem.ContentFolder;

            #region Old Stuff
            /*
            if (!folder.IsQuickAccess)
            {
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_AddFolderToQuickAccess_Title, 
                    AppResources.Alert_AddFolderToQuickAccess_Message, 
                    AppResources.AlertOption_Yes, 
                    AppResources.AlertOption_Cancel
                );

                if (answer)
                {
                    if (await App.Database.DoesQuickAccessFolderNameExistAsync(folder.Name))
                    {
                        (Option option, string newName) = await NameValidation.GetUniqueFolderName
                        (
                            this, 
                            folder.ParentID, 
                            AppResources.Prompt_FolderNameConflict_Title,
                            isQuickAccess: true,
                            message: AppResources.Prompt_QuickAccessFolderNameConflict_Message
                        );
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
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_RemoveFolderFromQuickAccess_Title, 
                    AppResources.Alert_RemoveFolderFromQuickAccess_Message, 
                    AppResources.AlertOption_Yes, 
                    AppResources.AlertOption_Cancel
                );

                if (answer)
                {
                    folder.IsQuickAccess = false;
                    await App.Database.SaveAsync(folder);
                    UpdateListView();
                }
            }*/
            #endregion

            if (!folder.IsQuickAccess)
            {
                if (await App.Database.DoesQuickAccessFolderNameExistAsync(folder.Name, folder.ID))
                {
                    FolderBeingProcessed = folder;
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Name Conflict Encountered",
                        "In order to add the folder to quick access, it must be renamed",
                        "OK",
                        "Cancel",
                        folder.ParentID,
                        folder.ID,
                        App.Database.DoesQuickAccessOrOtherwiseFolderNameExistAsync,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        folder.Name,
                        "Input folder name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameFolderThenAddToQuickAccess;
                    popup.CancelOptionClicked += CancelAddFolderToQuickAccess;
                    popup.BackgroundClicked += CancelAddFolderToQuickAccess;
                    popup.HardwareBackClicked += CancelAddFolderToQuickAccess;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    folder.IsQuickAccess = true;
                    folder.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(folder);
                    UpdateListView();
                }
            }
            else
            {
                folder.IsQuickAccess = false;
                folder.DateModified = DateTime.UtcNow;
                await App.Database.SaveAsync(folder);
                UpdateListView();
            }
        }

        private async void ProceedRenameFolderThenAddToQuickAccess(PromptPopupOptionEventArgs e)
        {

            await PopupNavigation.Instance.PopAsync();
            FolderBeingProcessed.Name = e.Text;
            FolderBeingProcessed.IsQuickAccess = true;
            FolderBeingProcessed.DateModified = DateTime.UtcNow;
            await App.Database.SaveAsync(FolderBeingProcessed);
            FolderBeingProcessed = null;
            UpdateListView();

        }

        private async void CancelAddFolderToQuickAccess(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            FolderBeingProcessed = null;
        }

        #endregion

        private async void Search_Clicked(object sender, EventArgs e)
        {
            var searchBar = new CustomSearchBar()
            {
                TextColor = Color.White,
                PlaceholderColor = Color.Gray,
                SearchIconColor = Color.White,
                CloseIconColor = Color.White,
                Placeholder = AppResources.SearchBar_SearchFolder_Placeholder
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

        //public async void HandleOptionClicked(PromptPopupPage sender, PromptPopupOptionEventArgs e)
        //{
        //    switch (e.Option)
        //    {
        //        case PopupOption.Left:
        //            await PopupNavigation.Instance.PushAsync(new AlertPopupPage("Left", e.Text, "OK"));
        //            break;
        //        case PopupOption.Right:
        //            await PopupNavigation.Instance.PushAsync(new AlertPopupPage("Right", e.Text, "Righty o"));
        //            break;
        //    }
        //}

        //private async void DisplayTestPopup(object sender, EventArgs e)
        //{
            
        //}
    }
}