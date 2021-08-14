using Notes.Services;
using Notes.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace Notes.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerPopupPage : PopupPage
    {
        public ColorPickerPopupPage()
        {
            InitializeComponent();
            BindingContext = new ColorPickerPopupPageViewModel(AppServiceProvider.Instance);
        }
    }
}