using System;
using System.IO;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Essentials;

using Notes.Data;
using Notes.Pages;
using Notes.Themes;
using Notes.Models;
using System.Threading.Tasks;
using System.Linq;

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
                    database = new NoteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NotesMasterPage();

            // Set theme from stored preferences, default to light if not previously set
            SetTheme(GetTheme());

            CreateDefaultStyleSheets();

        }

        public static int DefaultStyleSheetID = -1;

        void CreateDefaultStyleSheets()
        {
            DefaultStyleSheets = new List<CSS>();

            DefaultStyleSheets.Add(new CSS()
            {
                ID = -1,
                IsReadOnly = true,
                Name = "Default1 - Red (ReadOnly)",
                Text = "h1 { color: red; }"
            });

            DefaultStyleSheets.Add(new CSS()
            {
                ID = -2,
                IsReadOnly = true,
                Name = "Default2 - Blue (ReadOnly)",
                Text = "h1 { color: blue; }"
            });

            DefaultStyleSheets.Add(new CSS()
            {
                ID = -3,
                IsReadOnly = true,
                Name = "Default3 - Green (ReadOnly)",
                Text = "h1 { color: blue; }"
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

        public static void SetTheme(AppTheme theme)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case AppTheme.Dark:
                        mergedDictionaries.Add(new DarkTheme());
                        break;
                    case AppTheme.Light:
                    default: // this covers the 0 enum value AppTheme.Unspecified
                        mergedDictionaries.Add(new LightTheme());
                        break;
                }
            }
            Preferences.Set("Theme", (int)theme);
        }

        public static AppTheme GetTheme()
        {
            return (AppTheme)Preferences.Get("Theme", defaultValue: (int)AppInfo.RequestedTheme);
        }

        public static SortingMode GetSortingMode()
        {
            return (SortingMode)Preferences.Get("SortingMode", defaultValue: 0);
        }

        public static void SetSortingMode(SortingMode sortingMode)
        {
            Preferences.Set("SortingMode", (int)sortingMode);
        }

        private CSS GetNonUserStyleSheet(int id)
        {
            return DefaultStyleSheets.Where(i => i.ID == id).FirstOrDefault();
        }

        public static int GetStyleSheetID()
        {
            return Preferences.Get("StyleSheet", defaultValue: DefaultStyleSheetID);
        }

        public static void SetStyleSheetID(int id)
        {
            Preferences.Set("StyleSheet", id);
        }

        public async Task<CSS> GetStyleSheetAsync()
        {
            int sheetID = GetStyleSheetID();
            CSS sheet;

            if (sheetID > 0) // is user defined
            {
                sheet = await Database.GetSheetAsync(sheetID);
            }
            else
            {
                sheet = GetNonUserStyleSheet(sheetID);
            }

            return sheet;
        }

        public async Task<bool> DoesSheetNameExist(string name)
        {
            int defaultCount = DefaultStyleSheets.Where(i => i.Name == name).Count();
            return (defaultCount > 0) || (await Database.DoesCSSNameExist(name));
        }
    }
}
