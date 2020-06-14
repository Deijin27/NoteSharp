using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Themes;
using Xamarin.Essentials;
using System.IO;
using Notes.Models;

namespace Notes.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (App.Theme == AppTheme.Dark) ThemeSwitch.IsToggled = true;
            else ThemeSwitch.IsToggled = false;

            SpellCheckSwitch.IsToggled = App.IsSpellCheckEnabled;
        }

        private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                App.Theme = AppTheme.Dark;
            }
            else
            {
                App.Theme = AppTheme.Light;
            }
        }

        private void SpellCheckSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            App.IsSpellCheckEnabled = e.Value;
        }

        private async void CreateBackup()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();

            await DisplayAlert("External Storage Found", externalStoragePath, "OK");
            //string older = "NoteSharp";
            //string backupsFolder = "Backups";

            //string fileName = DateTime.UtcNow.ToString();



            //string folder = Path.Combine(externalStoragePath,)

            //string file = Path.Combine(externalStoragePath, "video.mp4");

            //while (!Directory.Exists())

        }

        private void CreateBackupButton_Clicked(object sender, EventArgs e)
        {
            CreateBackup();
        }
    }
}