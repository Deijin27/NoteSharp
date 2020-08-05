using System;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Resources;
using Notes.PopupPages;
using Rg.Plugins.Popup.Services;

namespace Notes.Pages
{
    public delegate void ChangesSavedEventHandler();

    public partial class NoteEntryPage : ContentPage
    {
        public bool NewNote = false;
        private string InitialText;
        private string InitialName;
        NotesPage PreviousPage;

        public Note CurrentNote { get; set; }

        public bool UnsavedChangesExist { get; set; }


        /// <summary>
        /// Use when loading an existing note
        /// </summary>
        /// <param name="note"></param>
        /// <param name="previousPage"></param>
        public NoteEntryPage(Note note, NotesPage previousPage)
        {
            PreviousPage = previousPage;
            InitialText = string.Copy(note.Text);
            InitialName = string.Copy(note.Name);

            InitializeComponent();
            ApplySettings();

            //NoteStorage = note;

            CurrentNote = note;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        public void UnfocusAll() 
        {
            // used to close keyboard in situations where it would stay open chen closing page
            // e.g. text is selected (no edits have been made) and close button is clicked.

            MainEditor.Unfocus();
            NameEntry.Unfocus();
        }

        /// <summary>
        /// Use for creating a new note
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="previousPage"></param>
        public NoteEntryPage(Guid folderID, NotesPage previousPage)
        {
            PreviousPage = previousPage;

            Note note = new Note()
            {
                FolderID = folderID 
            };

            InitialText = string.Copy(note.Text);
            InitialName = string.Copy(note.Name);
            NewNote = true;

            InitializeComponent();
            ApplySettings();

            CurrentNote = note;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        public void ApplySettings()
        {
            bool spellcheck = App.IsSpellCheckEnabled;
            MainEditor.IsSpellCheckEnabled = spellcheck;
            MainEditor.IsTextPredictionEnabled = spellcheck;
        }

        #region Close Clicked

        async void Close_Clicked(object sender, EventArgs e)
        {
            Note note = CurrentNote;

            if (UnsavedChangesExist)
            {
                var popup = new TwoOptionPopupPage
                (
                    "Save Changes?",
                    "Unsaved changes exist, would you like to save them?",
                    "Yes",
                    "No"
                );
                popup.LeftOptionClicked += SaveThenClose;
                popup.RightOptionClicked += ProceedWithClose;
                await PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                UnfocusAll();
                await Navigation.PopModalAsync();
            }

        }

        async void ProceedWithClose()
        {
            
            CurrentNote.Name = InitialName;
            CurrentNote.Text = InitialText;
            UnfocusAll();
            await PopupNavigation.Instance.PopAsync();
            await Navigation.PopModalAsync();
        }

        async void SaveThenClose()
        {
            await PopupNavigation.Instance.PopAsync();
            CloseAfter = true;
            Save();
        }

        #endregion

        #region Save Clicked
        void Save_Clicked(object sender, EventArgs e)
        {
            if (UnsavedChangesExist)
            {
                Save();
            }
        }

        async void Save()
        {
            await DisplayAlert("Debug: Save Clicked", "", "Proceed");
            Note note = CurrentNote;
            if (NewNote)
            {
                if (note.Name != InitialName)
                {

                    if (await App.Database.DoesNoteNameExistAsync(note.Name, note.FolderID, note.ID))
                    {
                        var popup = new UniqueNamePromptPopupPage
                        (
                            "Note name conflict",
                            "Input a different name for the note",
                            "OK",
                            "Cancel",
                            note.FolderID,
                            note.ID,
                            App.Database.DoesNoteNameExistAsync,
                            "Name invalid, it contains one of" + " \"/*.~",
                            "Name conflict encountered; try a different name",
                            "",
                            "Input note name..."
                        );
                        popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                        popup.CancelOptionClicked += CancelSaveNote;
                        popup.BackgroundClicked += CancelSaveNote;
                        popup.HardwareBackClicked += CancelSaveNote;

                        await PopupNavigation.Instance.PushAsync(popup);
                    }
                    else
                    {
                        note.DateModified = DateTime.UtcNow;
                        note.DateCreated = note.DateModified;
                        await App.Database.SaveAsync(note);
                        ChangesSaved?.Invoke();
                        if (CloseAfter)
                        {
                            await Navigation.PopModalAsync();
                        }
                        else
                        {
                            InitialName = note.Name;
                            InitialText = note.Text;
                            UnsavedChangesExist = false;
                        }
                    }
                }
                else
                {
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Note name",
                        "Input a name for the note",
                        "OK",
                        "Cancel",
                        note.FolderID,
                        note.ID,
                        App.Database.DoesNoteNameExistAsync,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        "",
                        "Input note name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                    popup.CancelOptionClicked += CancelSaveNote;
                    popup.BackgroundClicked += CancelSaveNote;
                    popup.HardwareBackClicked += CancelSaveNote;

                    await PopupNavigation.Instance.PushAsync(popup);
                }

            }
            else
            {
                UniqueNamePromptPopupPage.DoesNameExist namechecker;
                if (note.IsQuickAccess) namechecker = App.Database.DoesQuickAccessOrOtherwiseNoteNameExistAsync;
                else namechecker = App.Database.DoesNoteNameExistAsync;

                if (note.Name != InitialName && (await namechecker.Invoke(note.Name, note.FolderID, note.ID)))
                {
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Note name conflict",
                        "Input a different name for the note",
                        "OK",
                        "Cancel",
                        note.FolderID,
                        note.ID,
                        namechecker,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        "",
                        "Input note name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                    popup.CancelOptionClicked += CancelSaveNote;
                    popup.BackgroundClicked += CancelSaveNote;
                    popup.HardwareBackClicked += CancelSaveNote;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    note.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(note);
                    ChangesSaved?.Invoke();
                    if (CloseAfter)
                    {
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        InitialName = note.Name;
                        InitialText = note.Text;
                        UnsavedChangesExist = false;
                    }
                }
            }
        }

        bool CloseAfter = false;
        public async void ProceedRenameAndSaveNote(PromptPopupOptionEventArgs e)
        {
            Note note = CurrentNote;
            note.Name = e.Text;
            note.DateModified = DateTime.UtcNow;
            note.DateCreated = note.DateModified;
            await PopupNavigation.Instance.PopAsync();
            await App.Database.SaveAsync(note);
            ChangesSaved?.Invoke();

            if (CloseAfter)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                InitialName = note.Name;
                InitialText = note.Text;
                UnsavedChangesExist = false;
            }
        }

        public async void CancelSaveNote(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public event ChangesSavedEventHandler ChangesSaved;

        async void HtmlPreview_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HtmlPreviewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        async void MarkdownPreview_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MarkdownPreviewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        async void MarkdownView_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MarkdownViewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }

        private void MainEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }
    }
}
 