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
            Console.WriteLine("DEBUG: NoteEntryPage constructor called."); // DEBUG
            InitializeComponent();
        }

        /*        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Note note = (Note)BindingContext;
            note.FolderID = FolderID;

            if (NewNote)
            {
                string result = await DisplayPromptAsync("Name", "Input name for file");

                if (!string.IsNullOrWhiteSpace(result))
                {
                    while (await App.Database.DoesNoteNameExistAsync(result, note.FolderID))
                    {
                        result = await DisplayPromptAsync
                        (
                            "Name",
                            "A file of that name already exists in the current folder; please input a different name",
                            initialValue: result
                        );

                        if (string.IsNullOrWhiteSpace(result)) break;
                    }

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        note.Name = result;
                        note.DateModified = DateTime.UtcNow;
                        note.DateCreated = note.DateModified;
                        await App.Database.SaveNoteAsync(note);
                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                note.DateModified = DateTime.UtcNow;
                await App.Database.SaveNoteAsync(note);
                await Navigation.PopAsync();
            }
        }*/

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
            await Navigation.PushAsync(new MarkdownViewPage(markdownText));
        }
    }
}