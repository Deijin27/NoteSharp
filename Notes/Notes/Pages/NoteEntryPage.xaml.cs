using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Markdig;
using System.Threading.Tasks;
using Notes.Data;
using System.ComponentModel.Design;

namespace Notes.Pages
{

    public partial class NoteEntryPage : ContentPage
    {
        public bool NewNote = false;
        private string InitialText;
        private string InitialName;
        NotesPage PreviousPage;
        //Note NoteStorage;

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

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    BindingContext = NoteStorage;
        //}

        public void UnfocusAll() 
        {
            // used to close keyboard in situations where it would stay open chen closing page
            // e.g. text is selected (no edits have been made) and close button is clicked.

            MainEditor.Unfocus();
            NameEntry.Unfocus();
        }

        public NoteEntryPage(int folderID, NotesPage previousPage)
        {
            PreviousPage = previousPage;

            Note note = new Note() { FolderID = folderID };

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
                bool answer = await DisplayAlert("Exit?", "Exit without saving changes?", "Yes", "No");
                if (answer)
                {
                    await NavigateBack();
                }
            }
            else
            {
                UnfocusAll();
                await NavigateBack();
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
                        (option, newName) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Note Name Conflict",
                            message: "A note of the same name already exists in the current folder, please input a different name");
                        if (option == Option.OK)
                        {
                            note.Name = newName;
                            note.DateModified = DateTime.UtcNow;
                            await App.Database.SaveNoteAsync(note);
                            await NavigateBack();
                        }
                    }
                    else
                    {
                        note.DateModified = DateTime.UtcNow;
                        await App.Database.SaveNoteAsync(note);
                        UnfocusAll();
                        await NavigateBack();
                    }
                }
                else
                { 
                    (option, newName) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Name Note");
                    if (option == Option.OK)
                    {
                        note.Name = newName;
                        note.DateModified = DateTime.UtcNow;
                        note.DateCreated = note.DateModified;
                        await App.Database.SaveNoteAsync(note);
                        await NavigateBack();
                    }
                }

            }
            else
            {
                if (note.Name != InitialName && (await App.Database.DoesNoteNameExistAsync(note.Name, note.FolderID)))
                {
                    (option, newName) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Note Name Conflict",
                        message: "A note of the same name already exists in the current folder, please input a different name");
                    if (option == Option.OK)
                    {
                        note.Name = newName;
                        note.DateModified = DateTime.UtcNow;
                        await App.Database.SaveNoteAsync(note);
                        await NavigateBack();
                    }
                }
                else
                {
                    note.DateModified = DateTime.UtcNow;
                    await App.Database.SaveNoteAsync(note);
                    UnfocusAll();
                    await NavigateBack();
                }
            }
        }

        private async Task NavigateBack()
        {
            // thiss is weird but necessary, the reason weridly specific
            // When you close the app (by that I just mean press the home button or something)
            // then open it again, then click save, you are pushed to the NotesPage
            // but for some reason the notes page doesn't update
            // this means the note changes are saved, but if you click on 
            // the same note before doing something to force an update
            // it will have the old content when you look at it.
            // plus it will of course be listed as the wrong name
            // if you changed the name
            await PreviousPage.UpdateListView();
            await Navigation.PopModalAsync();
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        string NoteMarkdownToHtml()
        {
            var note = (Note)BindingContext;
            string markdownText = note.Text;

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();

            string html = Markdown.ToHtml(markdownText, pipeline);

            return html;
        }

        async void HtmlPreview_Clicked(object sender, EventArgs e)
        {
            string htmlString = await Task.Run(() => NoteMarkdownToHtml());
            await Navigation.PushAsync(new HtmlPreviewPage(htmlString));
        }

        async void MarkdownView_Clicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            //NoteStorage = note;

            string markdownText = note.Text;
            await Navigation.PushAsync(new MarkdownViewPage(markdownText, note.FolderID));
        }

        //private async void RenameNote_Clicked(object sender, EventArgs e)
        //{
        //    var note = (Note)BindingContext;

        //    (Option option, string result) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Rename Note",
        //        isQuickAccess: note.IsQuickAccess,
        //        initialValue: note.Name);
        //    if (option == Option.OK)
        //    {
        //        note.Name = result;
        //        BindingContext = note;
        //    }
        //}
    }
}