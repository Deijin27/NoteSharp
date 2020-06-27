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
        public delegate void BackupRestoredEventHandler();

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
                { PinkAccentColorRadioCircle, AppAccentColor.Pink },
                { PurpleAccentColorRadioCircle, AppAccentColor.Purple },
                { DeepPurpleAccentColorRadioCircle, AppAccentColor.DeepPurple },
                { IndigoAccentColorRadioCircle, AppAccentColor.Indigo },
                { BlueAccentColorRadioCircle, AppAccentColor.Blue },
                { LightBlueAccentColorRadioCircle, AppAccentColor.LightBlue },
                { CyanAccentColorRadioCircle, AppAccentColor.Cyan },
                { TealAccentColorRadioCircle, AppAccentColor.Teal },
                { GreenAccentColorRadioCircle, AppAccentColor.Green },
                { LightGreenAccentColorRadioCircle, AppAccentColor.LightGreen },
                { LimeAccentColorRadioCircle, AppAccentColor.Lime },
                { YellowAccentColorRadioCircle, AppAccentColor.Yellow },
                { AmberAccentColorRadioCircle, AppAccentColor.Amber },
                { OrangeAccentColorRadioCircle, AppAccentColor.Orange },
                { DeepOrangeAccentColorRadioCircle, AppAccentColor.DeepOrange }
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
            RestoredBackupEvent?.Invoke();
        }


        public event BackupRestoredEventHandler RestoredBackupEvent;
    }
}