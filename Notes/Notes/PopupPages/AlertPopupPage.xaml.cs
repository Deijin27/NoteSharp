using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Data;

namespace Notes.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertPopupPage : PopupPage
    {
        public AlertPopupPage(string title, string message, string dismissButtonText)
        {
            InitializeComponent();

            Title = title;
            Message = message;
            DismissButtonText = dismissButtonText;
        }

        public static new readonly BindableProperty TitleProperty = BindableProperty.Create
        (
            nameof(Title),
            typeof(string),
            typeof(AlertPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }

            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.Create
        (
            nameof(Message),
            typeof(string),
            typeof(AlertPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }

            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty DismissButtonTextProperty = BindableProperty.Create
        (
            nameof(DismissButtonText),
            typeof(string),
            typeof(AlertPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string DismissButtonText
        {
            get { return (string)GetValue(DismissButtonTextProperty); }

            set { SetValue(DismissButtonTextProperty, value); }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == TitleProperty.PropertyName)
            {
                titleLabel.Text = Title;
            }
            else if (propertyName == MessageProperty.PropertyName)
            {
                messageLabel.Text = Message;
            }
            else if (propertyName == DismissButtonTextProperty.PropertyName)
            {
                dismissButton.Text = DismissButtonText;
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
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}