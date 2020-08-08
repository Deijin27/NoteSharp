﻿using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
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

            MarkdownText = markdownText;
            InitialiseHtml(folderID);
        }

        public MarkdownViewPage(string html, CSS css) // for testing css
        {
            InitializeComponent();
            ToolbarItems.Remove(SelectCSS);

            HtmlText = html;
            UpdateWebView(css);
        }

        async void InitialiseHtml(Guid folderID)
        {
            (string markdownFinal, ErrorEncountered errorEncountered) = await MarkdownBuilder.BuildMarkdown(MarkdownText, this, folderID);

            if (errorEncountered == ErrorEncountered.True)
            {
                await Navigation.PopAsync();
            }
            else 
            {
                HtmlText = MarkdownBuilder.BuildHtml(markdownFinal);
                UpdateWebView();
            }
        }

        async void UpdateWebView(CSS sheet = null)
        {
            activityIndicator.IsRunning = true;
            string html = HtmlText;

            if (sheet == null) sheet = await((App)App.Current).GetStyleSheetAsync();
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
            var page = new StyleSheetSelectionPage();
            page.StyleSheetSelected += UpdateWebView;
            await Navigation.PushAsync(page);
        }

        private async void MarkdownWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
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