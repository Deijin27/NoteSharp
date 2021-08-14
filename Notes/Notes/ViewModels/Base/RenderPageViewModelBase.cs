using Notes.Models;
using Notes.Services;
using System;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public class RenderPageViewModelBase : ViewModelBase
    {
        string HtmlText;
        protected readonly IAppServiceProvider Services;

        public RenderPageViewModelBase(IAppServiceProvider services)
        {
            Services = services;
        }

        private async protected void ParseMarkdownThenUpdate(string markdownText, CSS css)
        {
            HtmlText = await Services.MarkdownBuilder.BuildHtml(markdownText);
            UpdateWebView(css);
        }

        private bool _activityIndicatorIsRunning = true;
        public bool ActivityIndicatorIsRunning
        {
            get => _activityIndicatorIsRunning;
            set => RaiseAndSetIfChanged(ref _activityIndicatorIsRunning, value);
        }

        private HtmlWebViewSource _webViewSource;
        public HtmlWebViewSource WebViewSource
        {
            get => _webViewSource;
            set => RaiseAndSetIfChanged(ref _webViewSource, value);
        }

        async protected void InitialiseHtml(string markdownText, Guid folderID)
        {
            (string markdownFinal, bool errorEncountered) = await Services.MarkdownBuilder.BuildMarkdown(markdownText, folderID);

            if (errorEncountered)
            {
                await Services.Navigation.GoBackAsync();
                ActivityIndicatorIsRunning = false;
            }
            else
            {
                HtmlText = await Services.MarkdownBuilder.BuildHtml(markdownFinal);
                UpdateWebView();
            }
        }

        async void UpdateWebView(CSS sheet = null)
        {
            ActivityIndicatorIsRunning = true;
            string html = HtmlText;

            if (sheet == null) sheet = await Services.StyleSheets.GetStyleSheetAsync(Services.Preferences.StyleSheetID);
            string css = sheet.Text;

            html = "<style>\n" + css + "\n</style>\n" + html; // maybe add some <body> tags so that the user can affect the whole thing with styles.
            var htmlSource = new HtmlWebViewSource
            {
                Html = html
            };
            WebViewSource = htmlSource;
            ActivityIndicatorIsRunning = false;
        }
    }
}
