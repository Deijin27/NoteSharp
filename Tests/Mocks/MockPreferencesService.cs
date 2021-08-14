using Notes;
using Notes.AccentColors;
using Notes.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Tests.Mocks
{
    public class MockPreferencesService : IPreferencesService
    {
        public AppAccentColor AccentColor { get; set; }
        public Color ColorPickerLastCopied { get; set; }
        public bool IsSpellCheckEnabled { get; set; }
        public SortingMode SortingMode { get; set; }
        public Guid StyleSheetID { get; set; }
        public AppTheme Theme { get; set; }
        public string Uri { get; set; }
    }
}
