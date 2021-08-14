using Xamarin.Forms;
using Notes.ViewModels;
using Notes.Services;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CssSelectionPage : ContentPage
    {
        public CssSelectionPage()
        {
            InitializeComponent();
            BindingContext = new CssSelectionPageViewModel(AppServiceProvider.Instance);
        }

    }
}