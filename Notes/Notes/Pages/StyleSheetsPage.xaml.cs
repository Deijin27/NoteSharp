using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;

namespace Notes.Pages
{
    public class StyleSheetDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CSSTemplate { get; set; }
        public DataTemplate CSSTemplate_ReadOnly { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var sheet = (CSS)item;

            if (sheet.IsReadOnly)
            {
                return CSSTemplate_ReadOnly;
            }
            return CSSTemplate;
        }
    }

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
                listView.SelectedItem = null;
            }
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            CSS sheet = mi.CommandParameter as CSS;

            bool answer = await DisplayAlert("Delete?", "Permanently delete style sheet?", "Yes", "No");
            if (answer)
            {
                await App.Database.DeleteSheetAsync(sheet);
                await UpdateListView();
            }
        }

        private async void Rename_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Rename_Clicked", "you did it!", "OK");
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}