using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.ViewModels;
using Notes.Services;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CssPage : ContentPage
    {
        public CssPage()
        {
            InitializeComponent();
            BindingContext = new CssPageViewModel(AppServiceProvider.Instance);
        }
    }
        
}