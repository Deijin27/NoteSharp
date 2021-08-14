using Notes.ViewModels;
using Notes.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickAccessPage : ContentPage
    {
        public QuickAccessPage()
        {
            InitializeComponent();
            BindingContext = new QuickAccessPageViewModel(AppServiceProvider.Instance);
        }
    }
}