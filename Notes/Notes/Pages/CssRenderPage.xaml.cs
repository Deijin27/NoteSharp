using Xamarin.Forms;
using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CssRenderPage : ContentPage
    {
        public CssRenderPage()
        {
            InitializeComponent();
            BindingContext = new CssRenderPageViewModel(AppServiceProvider.Instance);
        }

        private void MarkdownWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}