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
        //Note NoteStorage;

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

            BindingContext = note;
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

            BindingContext = note;
        }

        public void ApplySettings()
        {
            bool spellcheck = App.IsSpellCheckEnabled;
            MainEditor.IsSpellCheckEnabled = spellcheck;
            MainEditor.IsTextPredictionEnabled = spellcheck;
        }

        #region Close Clicked

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;

            #region OldStuff
            /*if (note.Name != InitialName || note.Text != InitialText)
            {
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_ExitWithoutSaving_Title, 
                    AppResources.Alert_ExitWithoutSaving_Message, 
                    AppResources.AlertOption_Yes, 
                    AppResources.AlertOption_No
                );
                if (answer)
                {
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                UnfocusAll();
                await Navigation.PopModalAsync();
            }*/
            #endregion

            if (note.Name != InitialName || note.Text != InitialText)
            {
                var popup = new TwoOptionPopupPage
                (
                    "Close without saving?",
                    "Any changes made will be lost.",
                    "Yes",
                    "Cancel"
                );
                popup.LeftOptionClicked += ProceedWithClose;
                popup.RightOptionClicked += CancelClose;
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
            Note note = (Note)BindingContext;
            note.Name = InitialName;
            note.Text = InitialText;
            UnfocusAll();
            await PopupNavigation.Instance.PopAsync();
            await Navigation.PopModalAsync();
        }

        async void CancelClose()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        #region Save Clicked
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
        Note note = (Note)BindingContext;
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
                    UnfocusAll();
                    await Navigation.PopModalAsync();
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
                UnfocusAll();
                await Navigation.PopModalAsync();
            }
        }
        }

        public async void ProceedRenameAndSaveNote(PromptPopupOptionEventArgs e)
        {
            Note note = (Note)BindingContext;
            note.Name = e.Text;
            note.DateModified = DateTime.UtcNow;
            note.DateCreated = note.DateModified;
            await PopupNavigation.Instance.PopAsync();
            await App.Database.SaveAsync(note);
            ChangesSaved?.Invoke();
            await Navigation.PopModalAsync();
        }

        public async void CancelSaveNote(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public event ChangesSavedEventHandler ChangesSaved;

        async void HtmlPreview_Clicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            string markdownText = note.Text;
            await Navigation.PushAsync(new HtmlPreviewPage(markdownText, note.FolderID));
        }

        async void MarkdownPreview_Clicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            string markdownText = note.Text;
            await Navigation.PushAsync(new MarkdownPreviewPage(markdownText, note.FolderID));
        }

        async void MarkdownView_Clicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            string markdownText = note.Text;
            await Navigation.PushAsync(new MarkdownViewPage(markdownText, note.FolderID));
        }
    }
}
 