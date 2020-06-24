using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;

namespace Notes.Pages
{
    public delegate void StyleSheetSelectedEventHandler(CSS Sheet);

    public partial class StyleSheetSelectionPage : ContentPage
    {
        public StyleSheetSelectionPage()
        {
            InitializeComponent();
            UpdateListView();
        }

        public event StyleSheetSelectedEventHandler StyleSheetSelected;

        public async void UpdateListView()
        {
            List<CSS> listViewItems = await App.GetAllStyleSheetsAsync();
            listView.ItemsSource = listViewItems;
        }
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var sheet = e.SelectedItem as CSS;

                App.StyleSheetID = sheet.ID;

                StyleSheetSelected?.Invoke(sheet);

                await Navigation.PopAsync();
            }
        }
    }
}