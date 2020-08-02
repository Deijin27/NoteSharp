using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.ComponentModel;

namespace Notes.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPickerPopupPage : PopupPage, INotifyPropertyChanged
    {
        public delegate void OptionClicked();
        public ColorPickerPopupPage
        (
            string title, 
            string message, 
            string cancelButtonText, 
            string copyHexButtonText,
            string copyRgbaButtonText,
            Color initialColor
        )
        {
            Title = title;
            Message = message;
            CopyHexButtonText = copyHexButtonText;
            CopyRgbaButtonText = copyRgbaButtonText;
            CancelButtonText = cancelButtonText;

            currentColorValue = initialColor;

            InitializeComponent();

            BindingContext = this;
        }

        #region One time stuff

        public static new readonly BindableProperty TitleProperty = BindableProperty.Create
        (
            nameof(Title),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
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
            BindingMode.OneWay
        );

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }

            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty CancelButtonTextProperty = BindableProperty.Create
        (
            nameof(CancelButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }

            set { SetValue(CancelButtonTextProperty, value); }
        }

        public static readonly BindableProperty CopyHexButtonTextProperty = BindableProperty.Create
        (
            nameof(CopyHexButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string CopyHexButtonText
        {
            get { return (string)GetValue(CopyHexButtonTextProperty); }

            set { SetValue(CopyHexButtonTextProperty, value); }
        }

        public static readonly BindableProperty CopyRgbaButtonTextProperty = BindableProperty.Create
        (
            nameof(CopyRgbaButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string CopyRgbaButtonText
        {
            get { return (string)GetValue(CopyRgbaButtonTextProperty); }

            set { SetValue(CopyRgbaButtonTextProperty, value); }
        }

        #endregion


        #region Two way stuff
        Color currentColorValue;
        public Color CurrentColor
        {
            get => currentColorValue;
            set
            {
                currentColorValue = value;
                OnPropertyChanged(nameof(CurrentColor));
            }
        }

        public double RedValue
        {
            get => CurrentColor.R;
            set
            {
                CurrentColor = new Color(value, CurrentColor.G, CurrentColor.B, CurrentColor.A);
                OnPropertyChanged(nameof(RedText));
            }
        }

        public string RedText
        {
            get => ((int)(RedValue * 255)).ToString();
            set
            {
                RedValue = double.TryParse(value, out double i) ? i / 255 : 0;
                OnPropertyChanged(nameof(RedValue));
            }
        }

        public double GreenValue
        {
            get => CurrentColor.G;
            set
            {
                CurrentColor = new Color(CurrentColor.R, value, CurrentColor.B, CurrentColor.A);
                OnPropertyChanged(nameof(GreenText));
            }
        }

        public string GreenText
        {
            get => ((int)(GreenValue * 255)).ToString();
            set 
            { 
                GreenValue = double.TryParse(value, out double i) ? i / 255 : 0;
                OnPropertyChanged(nameof(GreenValue));
            }
        }

        public double BlueValue
        {
            get => CurrentColor.B;
            set
            {
                CurrentColor = new Color(CurrentColor.R, CurrentColor.G, value, CurrentColor.A);
                OnPropertyChanged(nameof(BlueText));
            }
        }

        public string BlueText
        {
            get => ((int)(BlueValue * 255)).ToString();
            set
            {
                BlueValue = double.TryParse(value, out double i) ? i / 255 : 0;
                OnPropertyChanged(nameof(BlueValue));
            }
        }

        public double AlphaValue
        {
            get => CurrentColor.A;
            set
            {
                CurrentColor = new Color(CurrentColor.R, CurrentColor.G, CurrentColor.B, value);
                OnPropertyChanged(nameof(AlphaText));
            }
        }

        public string AlphaText
        {
            get
            {

                string txt = Math.Round(AlphaValue, 2).ToString();
                return txt;
            }
            set
            {
                AlphaValue = double.TryParse(value, out double i) ? i : 1;
                OnPropertyChanged(nameof(AlphaValue));
            }
        }

        #endregion


        #region Override Stuff

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
            return false; //base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return true; //base.OnBackgroundClicked();
        }

        #endregion

        private void CopyHex_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
            string hex = CurrentColor.ToHex().Remove(1, 2);
            Clipboard.SetTextAsync(hex);
            PopupNavigation.Instance.PopAsync();
        }

        private void CopyRgba_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
            var rgba = $"rgba({RedText}, {GreenText}, {BlueText}, {AlphaText})";
            Clipboard.SetTextAsync(rgba);
            PopupNavigation.Instance.PopAsync();
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}