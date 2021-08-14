using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.ViewModels;
using Notes.Services;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovePage : ContentPage
    {
        public MovePage()
        {
            InitializeComponent();
            BindingContext = new MovePageViewModel(AppServiceProvider.Instance);
        }
    }
}