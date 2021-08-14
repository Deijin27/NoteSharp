using System;
using Notes.Resources;
using Notes.Services;
using System.Windows.Input;
using Xamarin.Forms;
using Notes.RouteUtil;
using System.Collections.Generic;

namespace Notes.ViewModels
{
    
    public class PreviewPageViewModel : QueryableViewModelBase<PreviewPageSetupParameters>
    {
        readonly IAppServiceProvider Services;
        public PreviewPageViewModel(IAppServiceProvider services)
        {
            Services = services;
            CopyCommand = new Command(Copy);
        }

        public async override void Setup(PreviewPageSetupParameters setupParameters) // could always change void to Task if it's playing up during tests
        {
            ActivityIndicatorIsRunning = true;

            PageTitle = setupParameters.Mode switch
            {
                PreviewPageMode.Markdown => "Markdown Preview",
                PreviewPageMode.Html => "HTML Preview",
                _ => throw new Exception("Invalid PreviewPageMode"),
            };

            string text = (await Services.NoteDatabase.GetCachedNote()).Text;
            
            (string finalText, bool errorEncountered) = await Services.MarkdownBuilder.BuildMarkdown(text, setupParameters.FolderId);

            if (errorEncountered)
            {
                await Services.Navigation.GoBackAsync();
            }
            else
            {
                if (setupParameters.Mode == PreviewPageMode.Html)
                {
                    finalText = await Services.MarkdownBuilder.BuildHtml(finalText);
                }
                EditorText = finalText;
            }
            ActivityIndicatorIsRunning = false;
        }

        private string _pageTitle = "";
        public string PageTitle
        {
            get => _pageTitle;
            set => RaiseAndSetIfChanged(ref _pageTitle, value);
        }


        private bool _activityIndicatorIsRunning = true;
        public bool ActivityIndicatorIsRunning
        {
            get => _activityIndicatorIsRunning;
            set => RaiseAndSetIfChanged(ref _activityIndicatorIsRunning, value);
        }

        private string _editorText = "";
        public string EditorText
        {
            get => _editorText;
            set => RaiseAndSetIfChanged(ref _editorText, value);
        }

        public ICommand CopyCommand { get; set; }

        private async void Copy()
        {
            await Services.Clipboard.SetTextAsync(EditorText);
            await Services.Popups.AlertPopup
            (
                "Text Copied",
                "All text copied to clipboard",
                AppResources.AlertOption_OK
            );
        }

        
    }
}
