using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Data;
using Notes.AccentColors;
using Notes.Controls;

namespace Notes.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private AccentColorRadioObjectGroup accentRadioGroup;

        public SettingsPage()
        {
            InitializeComponent();

            if (App.Theme == AppTheme.Dark) ThemeSwitch.IsToggled = true;
            else ThemeSwitch.IsToggled = false;
            ThemeSwitch.Toggled += ThemeSwitch_Toggled; // doing this here avoids updating theme when setting toggled state

            SpellCheckSwitch.IsToggled = App.IsSpellCheckEnabled;

            accentRadioGroup = new AccentColorRadioObjectGroup()
            {
                { RedAccentColorRadioCircle, AppAccentColor.Red },
                { BlueAccentColorRadioCircle, AppAccentColor.Blue },
                { GreenAccentColorRadioCircle, AppAccentColor.Green }
            };
            accentRadioGroup.InitializeSelected(App.AccentColor);
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
        
        private async void CreateBackupButton_Clicked(object sender, EventArgs e)
        {
            await Backup.GetPermissionAndCreateBackup(this);
        }

        private async void RestoreBackupButton_Clicked(object sender, EventArgs e)
        {
            await Backup.GetPermissionAndRestoreBackup(this);
        }
    
    }
}