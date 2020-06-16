using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Markdig;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Notes.Data;

namespace Notes.Pages
{
    public partial class MarkdownViewPage : ContentPage
    {
        string MarkdownText;
        string HtmlText;

        public MarkdownViewPage(string markdownText, Guid folderID, bool DisableCssSelector = false)
        {
            InitializeComponent();
            if (DisableCssSelector)
            {
                ToolbarItems.Remove(SelectCSS);
            }

            MarkdownText = markdownText;
            InitialiseHtml(folderID);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // this is here because it's the easiest way to update the page after the CSS Selection Page is closed.
            UpdateWebView();
        }

        async void InitialiseHtml(Guid folderID)
        {
            (string markdownFinal, ErrorEncountered errorEncountered) = await App.Database.InterpolateAndInputTemplatesAsync(MarkdownText, this, folderID);

            if (errorEncountered == ErrorEncountered.True)
            {
                await Navigation.PopAsync();
            }
            else 
            {
                MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();
                HtmlText = Markdown.ToHtml(markdownFinal, pipeline);

                //UpdateWebView();
            }

            //activityIndicator.IsRunning = false;
        }

        async void UpdateWebView()
        {
            activityIndicator.IsRunning = true;
            string html = HtmlText;

            CSS sheet = await ((App)App.Current).GetStyleSheetAsync();

            string css = sheet.Text;

            html = "<style>\n" + css + "\n</style>\n" + html; // maybe add some <body> tags so that the user can affect the whole thing with styles.
            var htmlSource = new HtmlWebViewSource
            {
                Html = html
            };
            MarkdownWebView.Source = htmlSource;
            activityIndicator.IsRunning = false;
        }

        async void SelectCSS_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StyleSheetSelectionPage());
        }

        private async void MarkdownWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            //// args.url will have new page url  
            //if (args.Url == "https://google.com")
            //{
            //    args.Cancel = true;
            //    await DisplayAlert("Info", "This link was restricted to open in this app.", "Ok");
            //}



            string url = e.Url;
            e.Cancel = true;
            try
            {
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
            }
            catch (Exception) { }
            
        }
    }
}