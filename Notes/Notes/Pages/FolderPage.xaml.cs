using Xamarin.Forms;
using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderPage : ContentPage
    {
        public FolderPage()
        {
            InitializeComponent();
            BindingContext = new FolderPageViewModel(AppServiceProvider.Instance);
        }
    }
}