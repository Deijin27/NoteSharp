using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviewPage : ContentPage
    {
        public PreviewPage()
        {
            InitializeComponent();
            BindingContext = new PreviewPageViewModel(AppServiceProvider.Instance);
        }
    }
}