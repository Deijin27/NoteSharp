using System;
using Xamarin.Forms;
using Notes.Data;
using Xamarin.Essentials;
using Notes.Resources;

namespace Notes.Pages
{
    public partial class MarkdownPreviewPage : ContentPage
    {
        public MarkdownPreviewPage(string text, Guid folderID)
        {
            InitializeComponent();
            InitialiseMarkdown(folderID, text);
            activityIndicator.IsRunning = false;
        }

        async void InitialiseMarkdown(Guid folderID, string markdownText)
        {
            (string markdownFinal, ErrorEncountered errorEncountered) = await MarkdownBuilder.BuildMarkdown(markdownText, this, folderID);

            if (errorEncountered == ErrorEncountered.True)
            {
                await Navigation.PopAsync();
            }
            else
            {
                MarkdownEditor.Text = markdownFinal;
            }
        }

        private async void CopyButton_Clicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(MarkdownEditor.Text);
            await DisplayAlert
            (
                AppResources.Alert_MarkdownCopied_Title, 
                AppResources.Alert_MarkdownCopied_Message, 
                AppResources.AlertOption_OK
            );
        }
    }
}