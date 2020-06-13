using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;

namespace Notes.Pages
{

    public partial class StyleSheetSelectionPage : ContentPage
    {
        public StyleSheetSelectionPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateListView();
        }

        public async Task UpdateListView()
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

                await Navigation.PopAsync();
            }
        }
    }
}