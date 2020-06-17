using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using Notes.Data;
using Notes.AccentColors;

namespace Notes.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (App.Theme == AppTheme.Dark) ThemeSwitch.IsToggled = true;
            else ThemeSwitch.IsToggled = false;
            ThemeSwitch.Toggled += ThemeSwitch_Toggled; // doing this here might avoid updating theme when i set the thing if that does happen

            SetAccentColorButtonChecked(App.AccentColor);

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
        
        private async void CreateBackupButton_Clicked(object sender, EventArgs e)
        {
            await Backup.GetPermissionAndCreateBackup(this);
        }

        private async void RestoreBackupButton_Clicked(object sender, EventArgs e)
        {
            await Backup.GetPermissionAndRestoreBackup(this);
        }

        private void AccentColorRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton chosen = sender as RadioButton;
            
            switch (chosen.Text)
            {
                case "Red":
                    App.AccentColor = AppAccentColor.Red;
                    break;
                case "Blue":
                default:
                    App.AccentColor = AppAccentColor.Blue;
                    break;
            }
        }

        private void RedAccentColorButton_Pressed(object sender, EventArgs e) => UpdateAccentColor(AppAccentColor.Red);
        private void BlueAccentColorButton_Pressed(object sender, EventArgs e) => UpdateAccentColor(AppAccentColor.Blue);

        private void UpdateAccentColor(AppAccentColor accentColor)
        {
            App.AccentColor = accentColor;
            SetAccentColorButtonChecked(accentColor);
        }

        private void SetAccentColorButtonChecked(AppAccentColor accentColor)
        {

            switch (accentColor)
            {
                case AppAccentColor.Blue:
                    BlueAccentColorCheck.IsVisible = true;
                    RedAccentColorCheck.IsVisible = false;
                    break;
                case AppAccentColor.Red:
                    BlueAccentColorCheck.IsVisible = false;
                    RedAccentColorCheck.IsVisible = true;
                    break;
            }
        }
    }
}