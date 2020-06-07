using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Markdig;
using System.Threading.Tasks;
using Notes.Data;

namespace Notes.Pages
{

    public partial class NoteEntryPage : ContentPage
    {
        public bool NewNote = false;
        public int FolderID = 0;

        public NoteEntryPage()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;
            note.FolderID = FolderID;

            if (NewNote)
            {
                (Option option, string result) = await NameValidation.GetUniqueNoteName(this, note.FolderID, "Name Note");
                    
                if (option == Option.OK)
                {
                    note.Name = result;
                    note.DateModified = DateTime.UtcNow;
                    note.DateCreated = note.DateModified;
                    await App.Database.SaveNoteAsync(note);
                    await Navigation.PopAsync();
                }
                
            }
            else
            {
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(note);
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
            await Navigation.PushAsync(new MarkdownViewPage(markdownText, FolderID));
        }
    }
}