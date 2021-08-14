using Notes.AccentColors;
using Xamarin.Essentials;

namespace Notes.Services
{
    public interface IThemeService
    {
        AppAccentColor AccentColor { get; set; }
        AppTheme Theme { get; set; }
        void UpdateMergedDictionaries();
    }
}