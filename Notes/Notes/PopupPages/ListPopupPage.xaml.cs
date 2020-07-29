using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using Notes.Models;
using System.ComponentModel;

namespace Notes.PopupPages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListPopupPage : PopupPage, INotifyPropertyChanged
    {
        public delegate void OptionClicked();
        public delegate void ListPopupOptionClicked(ListPopupPageItem SelectedItem);
        public ListPopupPage
        (
            string title,
            string message,
            string cancelButtonText,
            List<ListPopupPageItem> items
        )
        {
            try { 
            InitializeComponent();

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            CancelButton.Text = cancelButtonText;
            OptionListView.ItemsSource = items;
            }
            catch (System.InvalidCastException e)
            {
                PopupNavigation.Instance.PushAsync(new AlertPopupPage("Error in ListPopupPage constructor", e.Message, "OK"));
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            HardwareBackClicked?.Invoke();
            // Return true if you don't want to close this popup page when a back button is pressed
            return true; //base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            BackgroundClicked?.Invoke();
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false; //base.OnBackgroundClicked();
        }

        public new event OptionClicked BackgroundClicked;
        public event OptionClicked HardwareBackClicked;
        public event ListPopupOptionClicked ListOptionClicked;
        public event OptionClicked CancelClicked;

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            CancelClicked.Invoke();
        }

        private void OptionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {

            
            if (e.SelectedItem != null)
            {
                var item = e.SelectedItem as ListPopupPageItem;
                ListOptionClicked.Invoke(item);
            }
            }
            catch (Exception err)
            {
                PopupNavigation.Instance.PushAsync(new AlertPopupPage("Error, Process cancelled", err.Message, "OK"));
            }
        }
    }
}