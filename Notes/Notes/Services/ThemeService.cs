using Notes.AccentColors;
using Notes.Themes;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notes.Services
{
    public class ThemeService : IThemeService
    {
        public ThemeService(IAppServiceProvider services)
        {
            Services = services;
        }

        IAppServiceProvider Services { get; set; }

        public AppTheme Theme
        {
            get => Services.Preferences.Theme;
            set
            {
                Services.Preferences.Theme = value;
                UpdateMergedDictionaries();
            }
        }

        public AppAccentColor AccentColor
        {
            get => Services.Preferences.AccentColor;
            set
            {
                Services.Preferences.AccentColor = value;
                UpdateMergedDictionaries();
            }
        }

        public void UpdateMergedDictionaries()
        {
            AppTheme theme = Services.Preferences.Theme;
            AppAccentColor accentColor = Services.Preferences.AccentColor;

            ICollection<ResourceDictionary> mergedDictionaries = App.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case AppTheme.Dark:
                        mergedDictionaries.Add(new DarkTheme());

                        switch (accentColor)
                        {
                            case AppAccentColor.Red:
                                mergedDictionaries.Add(new RedAccentDark());
                                break;
                            case AppAccentColor.Pink:
                                mergedDictionaries.Add(new PinkAccentDark());
                                break;
                            case AppAccentColor.Purple:
                                mergedDictionaries.Add(new PurpleAccentDark());
                                break;
                            case AppAccentColor.DeepPurple:
                                mergedDictionaries.Add(new DeepPurpleAccentDark());
                                break;
                            case AppAccentColor.Indigo:
                                mergedDictionaries.Add(new IndigoAccentDark());
                                break;
                            case AppAccentColor.Blue:
                                mergedDictionaries.Add(new BlueAccentDark());
                                break;
                            case AppAccentColor.LightBlue:
                                mergedDictionaries.Add(new LightBlueAccentDark());
                                break;
                            case AppAccentColor.Cyan:
                                mergedDictionaries.Add(new CyanAccentDark());
                                break;
                            case AppAccentColor.Teal:
                                mergedDictionaries.Add(new TealAccentDark());
                                break;
                            case AppAccentColor.Green:
                                mergedDictionaries.Add(new GreenAccentDark());
                                break;
                            case AppAccentColor.LightGreen:
                                mergedDictionaries.Add(new LightGreenAccentDark());
                                break;
                            case AppAccentColor.Lime:
                                mergedDictionaries.Add(new LimeAccentDark());
                                break;
                            case AppAccentColor.Yellow:
                                mergedDictionaries.Add(new YellowAccentDark());
                                break;
                            case AppAccentColor.Amber:
                                mergedDictionaries.Add(new AmberAccentDark());
                                break;
                            case AppAccentColor.Orange:
                                mergedDictionaries.Add(new OrangeAccentDark());
                                break;
                            case AppAccentColor.DeepOrange:
                                mergedDictionaries.Add(new DeepOrangeAccentDark());
                                break;
                            default:
                                goto case AppAccentColor.Blue;
                        }
                        break;

                    case AppTheme.Light:
                    default: // this covers the 0 enum value AppTheme.Unspecified
                        mergedDictionaries.Add(new LightTheme());
                        switch (accentColor)
                        {
                            case AppAccentColor.Red:
                                mergedDictionaries.Add(new RedAccentLight());
                                break;
                            case AppAccentColor.Pink:
                                mergedDictionaries.Add(new PinkAccentLight());
                                break;
                            case AppAccentColor.Purple:
                                mergedDictionaries.Add(new PurpleAccentLight());
                                break;
                            case AppAccentColor.DeepPurple:
                                mergedDictionaries.Add(new DeepPurpleAccentLight());
                                break;
                            case AppAccentColor.Indigo:
                                mergedDictionaries.Add(new IndigoAccentLight());
                                break;
                            case AppAccentColor.Blue:
                                mergedDictionaries.Add(new BlueAccentLight());
                                break;
                            case AppAccentColor.LightBlue:
                                mergedDictionaries.Add(new LightBlueAccentLight());
                                break;
                            case AppAccentColor.Cyan:
                                mergedDictionaries.Add(new CyanAccentLight());
                                break;
                            case AppAccentColor.Teal:
                                mergedDictionaries.Add(new TealAccentLight());
                                break;
                            case AppAccentColor.Green:
                                mergedDictionaries.Add(new GreenAccentLight());
                                break;
                            case AppAccentColor.LightGreen:
                                mergedDictionaries.Add(new LightGreenAccentLight());
                                break;
                            case AppAccentColor.Lime:
                                mergedDictionaries.Add(new LimeAccentLight());
                                break;
                            case AppAccentColor.Yellow:
                                mergedDictionaries.Add(new YellowAccentLight());
                                break;
                            case AppAccentColor.Amber:
                                mergedDictionaries.Add(new AmberAccentLight());
                                break;
                            case AppAccentColor.Orange:
                                mergedDictionaries.Add(new OrangeAccentLight());
                                break;
                            case AppAccentColor.DeepOrange:
                                mergedDictionaries.Add(new DeepOrangeAccentLight());
                                break;
                            default:
                                goto case AppAccentColor.Blue;
                        }
                        break;
                }
            }
        }
    }
}
