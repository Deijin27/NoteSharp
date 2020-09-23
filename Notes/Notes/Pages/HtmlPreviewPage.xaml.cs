using System;
using Xamarin.Forms;
using Notes.Data;
using Xamarin.Essentials;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;

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
            (string markdownFinal, bool errorEncountered) = await MarkdownBuilder.BuildMarkdown(markdownText, folderID);

            if (errorEncountered)
            {
                await Navigation.PopAsync();
            }
            else
            {
                HtmlEditor.Text = MarkdownBuilder.BuildHtml(markdownFinal);
            }
        }
        private async void CopyButton_Clicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(HtmlEditor.Text);
            await PopupNavigation.Instance.PushAsync(new AlertPopupPage
            (
                AppResources.Alert_HtmlCopied_Title, 
                AppResources.Alert_HtmlCopied_Message, 
                AppResources.AlertOption_OK
            ));
        }
    }
}