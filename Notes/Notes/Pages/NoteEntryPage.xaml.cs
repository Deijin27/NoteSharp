using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Markdig;
using System.Threading.Tasks;

namespace Notes.Pages
{
    public partial class NoteEntryPage : ContentPage
    {
        public bool NewNote = false;
        public int FolderID = 0;

        public NoteEntryPage()
        {
            Console.WriteLine("DEBUG: NoteEntryPage constructor called."); // DEBUG
            InitializeComponent();
        }

        /*protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!newNote) activeNote = (Note)BindingContext;
            
            activeNote.DateAccessed = DateTime.UtcNow;

            Console.WriteLine("On Appearing is being executed.");
            if (!newNote)
            {
                await App.Database.SaveNoteAsync(activeNote);
                Console.WriteLine("Data Accessed Update executed.");
            }
        }*/

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("DEBUG: OnSaveButtonClicked executed.");  // DEBUG
            var note = (Note)BindingContext;
            note.DateModified = DateTime.UtcNow;
            if (NewNote)
            {
                note.DateCreated = note.DateModified;
            }

            // For now, infer note name from first line in Text
            string firstLine = new StringReader(note.Text).ReadLine();
            firstLine = Regex.Replace(firstLine, "[#*~_^$`]", string.Empty).Trim(); // remove markdown sytax characters
            note.Name = firstLine;

            //note.FileIcon = "FileIcon";
            note.FolderID = FolderID;
            await App.Database.SaveNoteAsync(note);
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete?", "The note will be deleted permanently.", "Delete", "Cancel");
            if (answer) 
            {
                var note = (Note)BindingContext;
                await App.Database.DeleteNoteAsync(note);
                await Navigation.PopAsync();
            }
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
            string markdownText = note.Text;
            await Navigation.PushAsync(new MarkdownViewPage(markdownText));
        }

        private void Rename_Clicked(object sender, EventArgs e)
        {

        }
    }
}