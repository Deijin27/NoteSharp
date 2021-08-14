using Xamarin.Forms;
using Notes.ViewModels;
using Notes.Services;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CssFolderPage : ContentPage
    {
        public CssFolderPage()
        {
            InitializeComponent();
            BindingContext = new CssFolderPageViewModel(AppServiceProvider.Instance);
        }
    }
}