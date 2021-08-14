using Notes.AccentColors;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Notes.Services
{
    public interface IPreferencesService
    {
        AppAccentColor AccentColor { get; set; }
        Color ColorPickerLastCopied { get; set; }
        bool IsSpellCheckEnabled { get; set; }
        SortingMode SortingMode { get; set; }
        Guid StyleSheetID { get; set; }
        AppTheme Theme { get; set; }
        string Uri { get; set; }

    }
}