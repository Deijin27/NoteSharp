using Notes.Models;
using Notes.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public abstract class CssListViewModelBase : ViewModelBase, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            UpdateListView();
        }

        protected readonly IAppServiceProvider Services;
        public CssListViewModelBase(IAppServiceProvider services)
        {
            Services = services;
        }

        private List<CSS> _listViewItems;
        public List<CSS> ListViewItems
        {
            get => _listViewItems;
            set => RaiseAndSetIfChanged(ref _listViewItems, value);
        }

        private CSS _listViewSelectedItem = null;
        public CSS ListViewSelectedItem
        {
            get => _listViewSelectedItem;
            set
            {
                if (value != null)
                {
                    OnListViewItemSelected(value);
                    RaisePropertyChanged();
                }
            }
        }

        protected async void UpdateListView()
        {
            ListViewItems = await Services.StyleSheets.GetAllStyleSheetsAsync();
        }

        protected abstract void OnListViewItemSelected(CSS value);

    }
}
