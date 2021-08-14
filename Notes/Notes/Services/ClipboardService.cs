using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Notes.Services
{
    public class ClipboardService : IClipboardService
    {
        public Task SetTextAsync(string text) => Clipboard.SetTextAsync(text);
        public Task<string> GetTextAsync() => Clipboard.GetTextAsync();
    }
}
