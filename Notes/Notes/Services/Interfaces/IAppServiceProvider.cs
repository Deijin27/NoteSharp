using System.IO.Abstractions;

namespace Notes.Services
{
    public interface IAppServiceProvider
    {
        IPreferencesService Preferences { get; }
        IThemeService Theme { get; }
        IStyleSheetService StyleSheets { get; }
        INoteDatabase NoteDatabase { get; }
        IPopupService Popups { get; }
        string DatabasePath { get; }
        IBackupService Backup { get; }
        string ExternalStoragePath { get; }
        IPermissionService Permissions { get; }
        IFileSystem FileSystem { get; }
        INavigationService Navigation { get; }
        IClipboardService Clipboard { get; }
        IMarkdownBuilder MarkdownBuilder { get; }

        void ResetDatabase();
    }
}
