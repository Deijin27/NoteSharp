using Notes.Models;
using Notes.Services;

namespace Notes.ViewModels
{
    public class CssSelectionPageViewModel : CssListViewModelBase
    {
        public CssSelectionPageViewModel(IAppServiceProvider services) : base(services) { }

        protected override async void OnListViewItemSelected(CSS value)
        {
            Services.Preferences.StyleSheetID = value.ID;
            await Services.Navigation.GoBackAsync();
        }
    }
}
