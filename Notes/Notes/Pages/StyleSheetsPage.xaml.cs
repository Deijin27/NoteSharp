using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;

namespace Notes.Pages
{
    public partial class StyleSheetsPage : ContentPage
    {
        public int FolderID;

        public StyleSheetsPage()
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


        async void OnCSSAddedClicked(object sender, EventArgs e)
        {
            CSS sheet = new CSS() { IsReadOnly = false };
            await Navigation.PushAsync(new StyleSheetEntryPage(sheet.IsReadOnly, NewSheet:true)
            {
                BindingContext = sheet
            });
        }
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var sheet = e.SelectedItem as CSS;

                await Navigation.PushAsync(new StyleSheetEntryPage(sheet.IsReadOnly)
                {
                    BindingContext = sheet,
                    OriginalSheetName = sheet.Name
                });
                
            }
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}