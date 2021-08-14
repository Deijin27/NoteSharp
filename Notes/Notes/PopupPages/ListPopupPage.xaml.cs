using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using Notes.Models;
using Notes.Services;
using System.Threading.Tasks;

namespace Notes.PopupPages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListPopupPage : PopupPage
    {
        public ListPopupPage
            (
            string title,
            string message,
            string cancelButtonText,
            IEnumerable<ListPopupPageItem> items
        )
        {
            InitializeComponent();

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            CancelButton.Text = cancelButtonText;
            OptionListView.ItemsSource = items;
            Result = new TaskCompletionSource<(ListPopupResult Choice, object SelectedItem)>();
        }

        protected override bool OnBackButtonPressed()
        {
            Choice = ListPopupResult.HardwareBack;
            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            Choice = ListPopupResult.Background;
            return base.OnBackgroundClicked();
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Choice = ListPopupResult.Cancel;
            AppServiceProvider.Instance.Popups.PopAsync();
        }

        private void OptionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Choice = ListPopupResult.ListItem;
                SelectedItem = (e.SelectedItem as ListPopupPageItem).AssociatedObject;
                AppServiceProvider.Instance.Popups.PopAsync();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Result.SetResult((Choice, SelectedItem));
        }

        object SelectedItem = null;
        ListPopupResult Choice;
        public TaskCompletionSource<(ListPopupResult Choice, object SelectedItem)> Result { get; }
    }
}