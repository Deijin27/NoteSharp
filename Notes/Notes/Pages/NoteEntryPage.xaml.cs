using System;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Resources;

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

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;
            if (note.Name != InitialName || note.Text != InitialText)
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
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;
            Option option;
            string newName;
            if (NewNote)
            {
                if (note.Name != InitialName)
                {

                    if (await App.Database.DoesNoteNameExistAsync(note.Name, note.FolderID))
                    { 
                        (option, newName) = await NameValidation.GetUniqueNoteName
                        (
                            this, 
                            note.FolderID, 
                            AppResources.Prompt_NoteNameConflict_Title,
                            message: AppResources.Prompt_NoteNameConflict_Message
                        );
                        if (option == Option.OK)
                        {
                            note.Name = newName;
                            note.DateModified = DateTime.UtcNow;
                            note.DateCreated = note.DateModified;
                            await App.Database.SaveAsync(note);
                            ChangesSaved?.Invoke();
                            await Navigation.PopModalAsync();
                        }
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
                    (option, newName) = await NameValidation.GetUniqueNoteName
                    (
                        this, 
                        note.FolderID, 
                        AppResources.Prompt_NameNote_Title
                    );
                    if (option == Option.OK)
                    {
                        note.Name = newName;
                        note.DateModified = DateTime.UtcNow;
                        note.DateCreated = note.DateModified;
                        await App.Database.SaveAsync(note);
                        ChangesSaved?.Invoke();
                        await Navigation.PopModalAsync();
                    }
                }

            }
            else
            {
                if (note.Name != InitialName && (await App.Database.DoesNoteNameExistAsync(note.Name, note.FolderID)))
                {
                    (option, newName) = await NameValidation.GetUniqueNoteName
                    (
                        this,
                        note.FolderID,
                        AppResources.Prompt_NoteNameConflict_Title,
                        message: AppResources.Prompt_NoteNameConflict_Message
                    );
                    if (option == Option.OK)
                    {
                        note.Name = newName;
                        note.DateModified = DateTime.UtcNow;
                        await App.Database.SaveAsync(note);
                        ChangesSaved?.Invoke();
                        await Navigation.PopModalAsync();
                    }
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