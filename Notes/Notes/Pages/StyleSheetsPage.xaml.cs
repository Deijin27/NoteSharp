using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.PopupPages;
using Rg.Plugins.Popup.Services;
using Notes.Resources;

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
            UpdateListView();
        }

        public async void UpdateListView()
        {
            List<CSS> listViewItems = await App.GetAllStyleSheetsAsync();
            listView.ItemsSource = listViewItems;
        }

        public void ChangesSavedHandler()
        {
            UpdateListView();
        }


        async void OnCSSAddedClicked(object sender, EventArgs e)
        {
            var page = new StyleSheetEntryPage();
            page.ChangesSaved += ChangesSavedHandler;
            await Navigation.PushModalAsync(new NavigationPage(page));
        }
        
        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var sheet = e.SelectedItem as CSS;

                var page = new StyleSheetEntryPage(sheet);
                page.ChangesSaved += ChangesSavedHandler;
                await Navigation.PushModalAsync(new NavigationPage(page));
                listView.SelectedItem = null;
            }
        }

        #region Delete Sheet
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            CSS sheet = mi.CommandParameter as CSS;
            SheetBeingProcessed = sheet;

            var popup = new TwoOptionPopupPage
            (
                "Delete?",
                "Permanently delete style sheet?",
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );
            popup.LeftOptionClicked += ProceedDeleteSheet;
            popup.RightOptionClicked += CancelDeleteSheet;
            popup.BackgroundClicked += CancelDeleteSheet;
            popup.HardwareBackClicked += CancelDeleteSheet;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private async void ProceedDeleteSheet()
        {
            await PopupNavigation.Instance.PopAsync();

            if (SheetBeingProcessed.ID == App.StyleSheetID)
            {
                App.StyleSheetID = App.DefaultStyleSheetID;
            }

            await App.Database.DeleteSheetAsync(SheetBeingProcessed);
            SheetBeingProcessed = null;
            UpdateListView();
        }

        private async void CancelDeleteSheet()
        {
            await PopupNavigation.Instance.PopAsync();
            SheetBeingProcessed = null;
        }

#endregion

        private async void Rename_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            CSS sheet = mi.CommandParameter as CSS;

            SheetBeingProcessed = sheet;

            var popup = new PromptPopupPage
            (
                "Rename Folder",
                "Input new name for sheet",
                "OK",
                "Cancel",
                sheet.Name,
                "Input sheet name..."
            );
            popup.LeftOptionClicked += ProceedRenameSheet;
            popup.RightOptionClicked += CancelRenameSheet;
            popup.BackgroundClicked += CancelRenameSheet;
            popup.HardwareBackClicked += CancelRenameSheet;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        CSS SheetBeingProcessed;

        async void ProceedRenameSheet(PromptPopupOptionEventArgs args)
        {
            await PopupNavigation.Instance.PopAsync();
            SheetBeingProcessed.Name = args.Text;
            await App.Database.SaveSheetAsync(SheetBeingProcessed);
            SheetBeingProcessed = null;
            UpdateListView();
        }

        async void CancelRenameSheet(PromptPopupOptionEventArgs args)
        {
            SheetBeingProcessed = null;
            await PopupNavigation.Instance.PopAsync();
        }
    }
}