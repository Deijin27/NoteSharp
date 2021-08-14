using System;
using System.IO;
using System.IO.Abstractions;
using Xamarin.Forms;

namespace Notes.Services
{
    public class AppServiceProvider : IAppServiceProvider
    {
        private static AppServiceProvider _instance;
        public static AppServiceProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppServiceProvider();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public AppServiceProvider()
        {
            Preferences = new PreferencesService(this);
            Theme = new ThemeService(this);
            StyleSheets = new StyleSheetService(this);
            Popups = new PopupService(this);
            NoteDatabase = new NoteDatabase(DatabasePath);
            Backup = new BackupService(this);
            Permissions = new PermissionService();
            FileSystem = new FileSystem();
            NavigationInstance = new NavigationService(this);
            Clipboard = new ClipboardService();
            MarkdownBuilder = new MarkdownBuilder(this);
        }

        public string ExternalStoragePath => DependencyService.Get<IDeviceFileSystem>().GetExternalStoragePath();
        public string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3");
        public void ResetDatabase()
        {
            NoteDatabase = new NoteDatabase(DatabasePath);
        }

        public IClipboardService Clipboard { get; }

        public IPreferencesService Preferences { get; }

        public IThemeService Theme { get; }

        public IStyleSheetService StyleSheets { get; }

        public INoteDatabase NoteDatabase { get; private set; }

        public INavigationService Navigation => NavigationInstance;
        public NavigationService NavigationInstance { get; }

        public IBackupService Backup { get; }

        public IPopupService Popups { get; }

        public IPermissionService Permissions { get; }

        public IFileSystem FileSystem { get; }

        public IMarkdownBuilder MarkdownBuilder { get; }
    }
}
