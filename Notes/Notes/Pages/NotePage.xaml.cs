using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : ContentPage
    {
        public NotePage()
        {
            InitializeComponent();
            BindingContext = new NotePageViewModel(AppServiceProvider.Instance);
        }

        protected override bool OnBackButtonPressed() // maskes sure the view model knows when hardware back is clicked
        {
            ((NotePageViewModel)BindingContext).OnHardwareBackPressed();
            return true;
        }
    }
}
 