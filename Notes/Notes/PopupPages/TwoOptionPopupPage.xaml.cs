using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.PopupPages
{

    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoOptionPopupPage : PopupPage
    {
        public delegate void OptionClicked();
        public TwoOptionPopupPage
        (
            string title, 
            string message, 
            string leftButtonText, 
            string rightButtonText
        )
        {
            InitializeComponent();

            Title = title;
            Message = message;
            LeftButtonText = leftButtonText;
            RightButtonText = rightButtonText;
        }

        public static new readonly BindableProperty TitleProperty = BindableProperty.Create
        (
            nameof(Title),
            typeof(string),
            typeof(PromptPopupPage),
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
            typeof(PromptPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }

            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty LeftButtonTextProperty = BindableProperty.Create
        (
            nameof(LeftButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string LeftButtonText
        {
            get { return (string)GetValue(LeftButtonTextProperty); }

            set { SetValue(LeftButtonTextProperty, value); }
        }

        public static readonly BindableProperty RightButtonTextProperty = BindableProperty.Create
        (
            nameof(RightButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string RightButtonText
        {
            get { return (string)GetValue(RightButtonTextProperty); }

            set { SetValue(RightButtonTextProperty, value); }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == TitleProperty.PropertyName)
            {
                TitleLabel.Text = Title;
            }
            else if (propertyName == MessageProperty.PropertyName)
            {
                MessageLabel.Text = Message;
            }
            else if (propertyName == LeftButtonTextProperty.PropertyName)
            {
                LeftButton.Text = LeftButtonText;
            }
            else if (propertyName == RightButtonTextProperty.PropertyName)
            {
                RightButton.Text = RightButtonText;
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

        public event OptionClicked LeftOptionClicked;
        public event OptionClicked RightOptionClicked;
        public new event OptionClicked BackgroundClicked;
        public event OptionClicked HardwareBackClicked;

        private void RightButton_Clicked(object sender, EventArgs e)
        {
            RightOptionClicked?.Invoke();
        }

        private void LeftButton_Clicked(object sender, EventArgs e)
        {
            LeftOptionClicked?.Invoke();
        }
    }
}