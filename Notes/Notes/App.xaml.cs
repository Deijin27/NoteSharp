﻿using System;
using System.IO;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Data;
using Notes.Pages;
using Notes.Themes;
using Notes.AccentColors;
using Notes.Models;
using System.Threading.Tasks;
using System.Linq;
using Notes.Resources;

namespace Notes
{

    public partial class App : Application
    {

        static NoteDatabase database; // The access level for class members and struct members, including nested classes and structs, is private by default.

        public List<CSS> DefaultStyleSheets;

        public static NoteDatabase Database // Then this is the public handler that wraps the lowercase database
        {
            get
            {
                if (database == null)
                {
                    database = new NoteDatabase(DatabasePath);
                }
                return database;
            }
        }

        public static void ResetDatabase()
        {
            database = new NoteDatabase(DatabasePath);
        }

        public static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3");

        public App()
        {
            InitializeComponent();
            MainPage = new NotesMasterPage();

            // Set theme from stored preferences, default to light if not previously set
            UpdateMergedDictionaries();

            CreateDefaultStyleSheets();
        }

        public static Guid DefaultStyleSheetID => DefaultStyleSheetGuids[0];

        public static List<Guid> DefaultStyleSheetGuids = new List<Guid>()
        {
            Guid.Parse("ac67e818-73bd-4d85-a269-12f78a3f46d7"),
            Guid.Parse("4ad15e4c-e1db-4ff5-a620-61af06060ff1")
        };

        void CreateDefaultStyleSheets()
        {
            DefaultStyleSheets = new List<CSS>();

            DefaultStyleSheets.Add(new CSS()
            {
                ID = DefaultStyleSheetGuids[0],
                IsReadOnly = true,
                Name = "Modern Light",
                Text = AppResources.ModernLightCSS
            });

            DefaultStyleSheets.Add(new CSS()
            {
                ID = DefaultStyleSheetGuids[1],
                IsReadOnly = true,
                Name = "Modern Dark",
                Text = AppResources.ModernDarkCSS
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts 
            base.OnStart();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            base.OnSleep();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            base.OnResume();
        }

        public static async Task<List<CSS>> GetAllStyleSheetsAsync()
        {
            var styleSheets = ((App)App.Current).DefaultStyleSheets;
            styleSheets = styleSheets.Concat(await App.Database.GetSheetsAsync()).ToList();
            return styleSheets;
        }

        public static bool IsSpellCheckEnabled
        {
            get
            {
                return Preferences.Get("IsSpellCheckEnabled", defaultValue: true);
            }
            set
            {
                Preferences.Set("IsSpellCheckEnabled", value);
            }
        }

        public static AppAccentColor AccentColor
        {
            get
            {
                return (AppAccentColor)Preferences.Get("AccentColor", defaultValue: (int)AppAccentColor.Blue);
            }
            set
            {
                Preferences.Set("AccentColor", (int)value);
                UpdateMergedDictionaries();
            }
        }

        public static AppTheme Theme
        {
            get 
            {
                return (AppTheme)Preferences.Get("Theme", defaultValue: (int)AppInfo.RequestedTheme);
            }

            set
            {
                Preferences.Set("Theme", (int)value);
                UpdateMergedDictionaries();
            }
        }

        private static void UpdateMergedDictionaries()
        {
            AppTheme theme = Theme;
            AppAccentColor accentColor = AccentColor;

            ICollection<ResourceDictionary> mergedDictionaries = Current.Resources.MergedDictionaries;
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

        public static SortingMode SortingMode
        {
            get
            {
                return (SortingMode)Preferences.Get("SortingMode", defaultValue: (int)SortingMode.Name);
            }

            set
            {
                Preferences.Set("SortingMode", (int)value);
            }
        }

        public static string ExternalStoragePath => DependencyService.Get<IFileSystem>().GetExternalStoragePath();

        private CSS GetNonUserStyleSheet(Guid id)
        {
            return DefaultStyleSheets.Where(i => i.ID == id).FirstOrDefault();
        }

        public static Guid StyleSheetID
        {
            get
            {
                return Guid.Parse(Preferences.Get("StyleSheet", defaultValue: DefaultStyleSheetID.ToString()));
            }

            set
            {
                Preferences.Set("StyleSheet", value.ToString());
            }
        }

        public async Task<CSS> GetStyleSheetAsync()
        {
            Guid sheetID = StyleSheetID;
            CSS sheet;

            if (!DefaultStyleSheetGuids.Contains(sheetID)) // is user defined
            {
                sheet = await Database.GetSheetAsync(sheetID);
            }
            else
            {
                sheet = GetNonUserStyleSheet(sheetID);
            }

            return sheet;
        }

        //public async Task<bool> DoesSheetNameExist(string name)
        //{
        //    int defaultCount = DefaultStyleSheets.Where(i => i.Name == name).Count();
        //    return (defaultCount > 0) || (await Database.DoesCSSNameExist(name));
        //}

        public static Color ColorPickerLastCopied
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
