using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Notes.PopupPages
{

    public class DoubleTo256Converter : IValueConverter
    {
        // number to string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)((double)value * 255)).ToString();
        }

        // string to number
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.TryParse((string)value, out double i) ? i / 255 : 0;
        }
    }

    public class AlphaConverter : IValueConverter
    {
        // number to string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((double)value, 2).ToString();
        }

        // string to number
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.TryParse((string)value, out double i) ? i : 1.0;
        }
    }


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
            string copyRgbaButtonText
        )
        {
            Title = title;
            Message = message;
            CopyHexButtonText = copyHexButtonText;
            CopyRgbaButtonText = copyRgbaButtonText;
            CancelButtonText = cancelButtonText;


            currentColorValue = App.ColorPickerLastCopied;

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


        bool ValueIsChanging = false;


        #region Two way stuff
        Color currentColorValue;
        public Color CurrentColor
        {
            get => currentColorValue;
            set
            {
                if (!ValueIsChanging)
                {
                    currentColorValue = value;
                    ValueIsChanging = true;
                    OnPropertyChanged(nameof(CurrentColor));
                    OnPropertyChanged(nameof(RedValue));
                    OnPropertyChanged(nameof(GreenValue));
                    OnPropertyChanged(nameof(BlueValue));
                    OnPropertyChanged(nameof(HueValue));
                    OnPropertyChanged(nameof(SaturationValue));
                    OnPropertyChanged(nameof(LuminosityValue));
                    OnPropertyChanged(nameof(AlphaValue));
                    ValueIsChanging = false;
                }
            }
        }

        public double RedValue
        {
            get => CurrentColor.R;
            set
            { 
                if (RedValue != value)
                {
                    CurrentColor = new Color(value, CurrentColor.G, CurrentColor.B, CurrentColor.A);
                    //OnPropertyChanged(nameof(RedValue));
                }
            }
        }

        public double GreenValue
        {
            get => CurrentColor.G;
            set
            {
                if (GreenValue != value)
                {
                    CurrentColor = new Color(CurrentColor.R, value, CurrentColor.B, CurrentColor.A);
                    //OnPropertyChanged(nameof(GreenValue));
                }
            }
        }

        public double BlueValue
        {
            get => CurrentColor.B;
            set
            {
                if (BlueValue != value)
                {
                    CurrentColor = new Color(CurrentColor.R, CurrentColor.G, value, CurrentColor.A);
                    //OnPropertyChanged(nameof(BlueValue));
                }
            }
        }

        public double HueValue
        {
            get => CurrentColor.Hue;
            set
            {
                if (HueValue != value)
                {
                    CurrentColor = CurrentColor.WithHue(value);
                    //OnPropertyChanged(nameof(HueValue));
                }
            }
        }


        public double SaturationValue
        {
            get => CurrentColor.Saturation;
            set
            {
                if (SaturationValue != value)
                {
                    CurrentColor = CurrentColor.WithSaturation(value);
                    //OnPropertyChanged(nameof(SaturationValue));
                }
            }
        }


        public double LuminosityValue
        {
            get => CurrentColor.Luminosity;
            set
            {
                if (LuminosityValue != value)
                {
                    CurrentColor = CurrentColor.WithLuminosity(value);
                    //OnPropertyChanged(nameof(LuminosityValue));
                }
            }
        }

        public double AlphaValue
        {
            get => CurrentColor.A;
            set
            {
                if (AlphaValue != value)
                {
                    CurrentColor = new Color(CurrentColor.R, CurrentColor.G, CurrentColor.B, value);
                }
                //OnPropertyChanged(nameof(AlphaText));
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


        private async void SetFromClipboard(object sender, EventArgs e)
        {
            string text = await Clipboard.GetTextAsync();
            text = text.Trim();
            if (Regex.IsMatch(text, @"^#([0-9A-F]{3}){1,2}$", RegexOptions.IgnoreCase))
            {
                CurrentColor = Color.FromHex(text);
            }
            else
            {
                var matches = Regex.Matches(text, @"^rgba\(\s*?(?<R>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*?,\s*?(?<G>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*?,\s*?(?<B>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*?,\s*?(?<A>(1(\.0+)?)|(0(\.\d+)?))\s*?\)$");
                if (matches.Count > 0)
                {
                    var groups = matches[0].Groups;
                    //Console.WriteLine("|{0}|{1}|{2}|{3}|{4}|", groups[0].Value, groups["R"].Value, groups["G"].Value, groups["B"].Value, groups["A"].Value);
                    //Console.WriteLine("|{0}|{1}|{2}|{3}|{4}|", groups[0].Value, int.Parse(groups["R"].Value), int.Parse(groups["G"].Value), int.Parse(groups["B"].Value), double.Parse(groups["A"].Value));
                    CurrentColor = new Color(double.Parse(groups["R"].Value)/255, double.Parse(groups["G"].Value)/255, double.Parse(groups["B"].Value)/255, double.Parse(groups["A"].Value));
                }
            }
        }


        private async void CopyHex_Clicked(object sender, EventArgs e)
        {
            string hex = CurrentColor.ToHex().Remove(1, 2);
            await Clipboard.SetTextAsync(hex);
            App.ColorPickerLastCopied = CurrentColor;
            await PopupNavigation.Instance.PopAsync();
        }

        private async void CopyRgba_Clicked(object sender, EventArgs e)
        {
            var conv = new DoubleTo256Converter();

            var rgba = $"rgba({(int)(RedValue * 255)}, {(int)(GreenValue * 255)}, {(int)(BlueValue * 255)}, {Math.Round(AlphaValue, 2)})";
            await Clipboard.SetTextAsync(rgba);
            App.ColorPickerLastCopied = CurrentColor;
            await PopupNavigation.Instance.PopAsync();
        }



        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}