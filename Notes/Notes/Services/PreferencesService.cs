using Notes.AccentColors;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notes.Services
{
    public class PreferencesService : IPreferencesService
    {
        public PreferencesService(IAppServiceProvider services)
        {
            Services = services;
        }

        IAppServiceProvider Services { get; set; }

        public AppTheme Theme
        {
            get => (AppTheme)Preferences.Get("Theme", defaultValue: (int)AppInfo.RequestedTheme);
            set => Preferences.Set("Theme", (int)value);
        }

        public AppAccentColor AccentColor
        {
            get => (AppAccentColor)Preferences.Get("AccentColor", defaultValue: (int)AppAccentColor.Blue);
            set => Preferences.Set("AccentColor", (int)value);
        }

        public bool IsSpellCheckEnabled
        {
            get => Preferences.Get("IsSpellCheckEnabled", defaultValue: true);
            set => Preferences.Set("IsSpellCheckEnabled", value);
        }

        public SortingMode SortingMode
        {
            get => (SortingMode)Preferences.Get("SortingMode", defaultValue: (int)SortingMode.Name);
            set => Preferences.Set("SortingMode", (int)value);
        }

        public Guid StyleSheetID
        {
            get => Guid.Parse(Preferences.Get("StyleSheet", defaultValue: Services.StyleSheets.DefaultStyleSheetID.ToString()));
            set => Preferences.Set("StyleSheet", value.ToString());
        }

        public string Uri
        {
            get => Preferences.Get("Uri", null);
            set => Preferences.Set("Uri", value);
        }

        public Color ColorPickerLastCopied
        {
            get
            {
                double red = Preferences.Get("ColorPickerLastCopied_Red", defaultValue: 0.5);
                double green = Preferences.Get("ColorPickerLastCopied_Green", defaultValue: 0.5);
                double blue = Preferences.Get("ColorPickerLastCopied_Blue", defaultValue: 0.5);
                double alpha = Preferences.Get("ColorPickerLastCopied_Alpha", defaultValue: 0.5);
                return new Color(red, green, blue, alpha);
            }

            set
            {
                Preferences.Set("ColorPickerLastCopied_Red", value.R);
                Preferences.Set("ColorPickerLastCopied_Green", value.G);
                Preferences.Set("ColorPickerLastCopied_Blue", value.B);
                Preferences.Set("ColorPickerLastCopied_Alpha", value.A);
            }
        }
    }
}
