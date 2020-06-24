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
        public DataTemplate CSSTemplate_NameOnly { get; set; }
        public DataTemplate CSSTemplate_ReadOnly { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var sheet = (CSS)item;

            if (sheet.IsReadOnly)
            {
                return CSSTemplate_ReadOnly;
            }
            return CSSTemplate_NameOnly;
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
            await Navigation.PushModalAsync(new NavigationPage(new StyleSheetEntryPage()));
        }
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var sheet = e.SelectedItem as CSS;

                await Navigation.PushModalAsync(new NavigationPage(new StyleSheetEntryPage(sheet)));
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
                if (sheet.ID == App.StyleSheetID)
                {
                    App.StyleSheetID = App.DefaultStyleSheetID;
                }
                await App.Database.DeleteSheetAsync(sheet);
                await UpdateListView();
            }
        }

        private async void Rename_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            CSS sheet = mi.CommandParameter as CSS;

            string newName = await DisplayPromptAsync("Rename Sheet", "Input new name for sheet", initialValue:sheet.Name);

            if (newName != null)
            {
                sheet.Name = newName;
                await App.Database.SaveSheetAsync(sheet);
                await UpdateListView();
            }
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}