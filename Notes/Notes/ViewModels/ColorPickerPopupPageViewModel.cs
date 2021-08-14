using Notes.Services;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public class ColorPickerPopupPageViewModel : ViewModelBase
    {
        readonly IAppServiceProvider Services;
        public ColorPickerPopupPageViewModel(IAppServiceProvider services)
        {
            Services = services;
            _currentColor = AppServiceProvider.Instance.Preferences.ColorPickerLastCopied;

            SetFromClipboardCommand = new Command(SetFromClipboard);
            CloseCommand = new Command(Close);
            CopyHexCommand = new Command(CopyHex);
            CopyRgbaCommand = new Command(CopyRgba);
        }

        #region Color Multiple Linked Sliders

        private bool currentColorValueIsChanging = false;
        Color _currentColor;
        public Color CurrentColor
        {
            get => _currentColor;
            set
            {
                if (!currentColorValueIsChanging)
                {
                    _currentColor = value;
                    currentColorValueIsChanging = true;
                    RaisePropertyChanged(nameof(CurrentColor));
                    RaisePropertyChanged(nameof(RedValue));
                    RaisePropertyChanged(nameof(GreenValue));
                    RaisePropertyChanged(nameof(BlueValue));
                    RaisePropertyChanged(nameof(HueValue));
                    RaisePropertyChanged(nameof(SaturationValue));
                    RaisePropertyChanged(nameof(LuminosityValue));
                    RaisePropertyChanged(nameof(AlphaValue));
                    currentColorValueIsChanging = false;
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
            }
        }

        #endregion
    
    
        public ICommand SetFromClipboardCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand CopyHexCommand { get; set; }
        public ICommand CopyRgbaCommand { get; set; }

        private async void SetFromClipboard()
        {
            string text = await Services.Clipboard.GetTextAsync();
            text = text.Trim();
            if (Regex.IsMatch(text, @"^#([0-9A-F]{3}){1,2}$", RegexOptions.IgnoreCase))
            {
                CurrentColor = Color.FromHex(text);
            }
            else
            {
                var matches = Regex.Matches(text, @"^rgba\(\s*?(?<R>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*,\s*(?<G>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*,\s*(?<B>[01]?[0-9][0-9]?|2[0-4][0-9]|25[0-5])\s*,\s*(?<A>(1(\.0+)?)|(0(\.\d+)?))\s*\)$");
                if (matches.Count > 0)
                {
                    var groups = matches[0].Groups;
                    CurrentColor = new Color(
                        double.Parse(groups["R"].Value) / 255, 
                        double.Parse(groups["G"].Value) / 255, 
                        double.Parse(groups["B"].Value) / 255, 
                        double.Parse(groups["A"].Value));
                }
            }
        }

        private async void CopyHex()
        {
            string hex = CurrentColor.ToHex().Remove(1, 2);
            await Services.Clipboard.SetTextAsync(hex);
            Services.Preferences.ColorPickerLastCopied = CurrentColor;
            await Services.Popups.PopAsync();
        }

        private async void CopyRgba()
        {
            var rgba = $"rgba({(int)(RedValue * 255)}, {(int)(GreenValue * 255)}, {(int)(BlueValue * 255)}, {Math.Round(AlphaValue, 2)})";
            await Services.Clipboard.SetTextAsync(rgba);
            Services.Preferences.ColorPickerLastCopied = CurrentColor;
            await Services.Popups.PopAsync();
        }

        private async void Close()
        {
            await Services.Popups.PopAsync();
        }
    }
}
