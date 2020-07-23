using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.PopupPages
{

    //public enum PromptPopupOption
    //{
    //    Left,
    //    Right,
    //    Background,
    //    HardwareBack
    //}

    public class PromptPopupOptionEventArgs
    {
        public PromptPopupOptionEventArgs(string text)
        {
            Text = text;
        }

        public readonly string Text;
        //public readonly PromptPopupOption Option;
    }

    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromptPopupPage : PopupPage
    {
        public delegate void OptionClicked(PromptPopupOptionEventArgs e);

        public PromptPopupPage
        (
            string title, 
            string message, 
            string leftButtonText, 
            string rightButtonText,
            string initialEntryText = "", 
            string placeholderText = ""
        )
        {
            InitializeComponent();

            Title = title;
            Message = message;
            LeftButtonText = leftButtonText;
            RightButtonText = rightButtonText;
            EntryText = initialEntryText;
            EntryPlaceholder = placeholderText;
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

        public static readonly BindableProperty EntryTextProperty = BindableProperty.Create
        (
            nameof(EntryText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string EntryText
        {
            get { return (string)GetValue(EntryTextProperty); }

            set { SetValue(EntryTextProperty, value); }
        }

        public static readonly BindableProperty EntryPlaceholderProperty = BindableProperty.Create
        (
            nameof(EntryPlaceholder),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.TwoWay
        );

        public string EntryPlaceholder
        {
            get { return (string)GetValue(EntryPlaceholderProperty); }

            set { SetValue(EntryPlaceholderProperty, value); }
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
            else if (propertyName == EntryTextProperty.PropertyName)
            {
                InputEntry.Text = EntryText;
            }
            else if (propertyName == LeftButtonTextProperty.PropertyName)
            {
                LeftButton.Text = LeftButtonText;
            }
            else if (propertyName == RightButtonTextProperty.PropertyName)
            {
                RightButton.Text = RightButtonText;
            }
            else if (propertyName == EntryPlaceholderProperty.PropertyName)
            {
                InputEntry.Placeholder = EntryPlaceholder;
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
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(EntryText);
            HardwareBackClicked?.Invoke(argsOut);
            // Return true if you don't want to close this popup page when a back button is pressed
            return true; //base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(EntryText);
            BackgroundClicked?.Invoke(argsOut);
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false; //base.OnBackgroundClicked();
        }

        private event OptionClicked LeftOptionClicked;
        private event OptionClicked RightOptionClicked;
        private new event OptionClicked BackgroundClicked;
        private event OptionClicked HardwareBackClicked;

        private void RightButton_Clicked(object sender, EventArgs e)
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(EntryText);
            RightOptionClicked?.Invoke(argsOut);
        }

        private void LeftButton_Clicked(object sender, EventArgs e)
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(EntryText);
            LeftOptionClicked?.Invoke(argsOut);
        }
    }
}