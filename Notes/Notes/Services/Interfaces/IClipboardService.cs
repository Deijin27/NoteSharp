using System.Threading.Tasks;

namespace Notes.Services
{
    public interface IClipboardService
    {
        Task<string> GetTextAsync();
        Task SetTextAsync(string text);
    }
}