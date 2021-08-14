using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RenderPage : ContentPage
    {
        public RenderPage()
        {
            InitializeComponent();
            BindingContext = new RenderPageViewModel(AppServiceProvider.Instance);
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