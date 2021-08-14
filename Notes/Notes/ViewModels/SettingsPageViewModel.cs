using Xamarin.Essentials;
using Xamarin.Forms;
using Notes.AccentColors;
using Notes.Services;
using System.Windows.Input;

namespace Notes.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public IAppServiceProvider Services;

        public SettingsPageViewModel(IAppServiceProvider services)
        {
            Services = services;

            CreateBackupCommand = new Command(async () => await Services.Backup.GetPermissionAndCreateBackup());
            RestoreBackupCommand = new Command(async () => await Services.Backup.GetPermissionAndRestoreBackup());
        }

        public bool IsDarkTheme
        {
            get => Services.Theme.Theme == AppTheme.Dark;
            set => RaiseAndSetIfChanged(IsDarkTheme, value, v => Services.Theme.Theme = v ? AppTheme.Dark : AppTheme.Light);
        }

        public bool IsSpellCheckEnabled
        {
            get => Services.Preferences.IsSpellCheckEnabled;
            set => RaiseAndSetIfChanged(IsSpellCheckEnabled, value, v => Services.Preferences.IsSpellCheckEnabled = v);
        }

        public AppAccentColor AccentColor
        {
            get => Services.Theme.AccentColor;
            set => RaiseAndSetIfChanged(AccentColor, value, v => Services.Theme.AccentColor = v);
        }

        public ICommand CreateBackupCommand { get; set; }
        public ICommand RestoreBackupCommand { get; set; }

    }
}
