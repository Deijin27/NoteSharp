using Notes.Services;
using System.IO.Abstractions;

namespace Tests.Mocks
{
    class MockAppServiceProvider : IAppServiceProvider
    {

        public MockPreferencesService MockPreferences = new MockPreferencesService();
        public IPreferencesService Preferences => MockPreferences;

        public IThemeService Theme { get; set; }


        public MockStyleSheetService MockStyleSheets = new MockStyleSheetService();
        public IStyleSheetService StyleSheets => MockStyleSheets;


        public MockNoteDatabase MockNoteDatabase = new MockNoteDatabase();
        public INoteDatabase NoteDatabase => MockNoteDatabase;


        public MockPopupService MockPopups = new MockPopupService();
        public IPopupService Popups => MockPopups;


        public string DatabasePath { get; set; }


        public IBackupService Backup { get; set; }


        public string ExternalStoragePath { get; set; }


        public IPermissionService Permissions { get; set; }


        public IFileSystem FileSystem { get; set; }


        public MockNavigationService MockNavigation = new MockNavigationService();
        public INavigationService Navigation => MockNavigation;


        public MockClipboardService MockClipboard = new MockClipboardService();
        public IClipboardService Clipboard => MockClipboard;


        public MockMarkdownBuilder MockMarkdownBuilder = new MockMarkdownBuilder();
        public IMarkdownBuilder MarkdownBuilder => MockMarkdownBuilder;


        public int ResetDatabaseCallCount = 0;
        public void ResetDatabase()
        {
            ResetDatabaseCallCount++;
        }
    }
}
