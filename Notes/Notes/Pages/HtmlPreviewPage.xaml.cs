using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Notes.Data;

namespace Notes.Pages
{
    public partial class HtmlPreviewPage : ContentPage
    {
        public HtmlPreviewPage(string text, Guid folderID)
        {
            InitializeComponent();
            InitialiseHtml(folderID, text);
            activityIndicator.IsRunning = false;
        }

        async void InitialiseHtml(Guid folderID, string markdownText)
        {
            (string markdownFinal, ErrorEncountered errorEncountered) = await MarkdownBuilder.BuildMarkdown(markdownText, this, folderID);

            if (errorEncountered == ErrorEncountered.True)
            {
                await Navigation.PopAsync();
            }
            else
            {
                HtmlEditor.Text = MarkdownBuilder.BuildHtml(markdownFinal);
            }
        }
    }
}