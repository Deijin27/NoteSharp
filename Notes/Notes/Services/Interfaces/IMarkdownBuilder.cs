using System;
using System.Threading.Tasks;

namespace Notes.Services
{
    public interface IMarkdownBuilder
    {
        Task<string> BuildHtml(string text);
        Task<(string result, bool errorEncountered)> BuildMarkdown(string text, Guid folderId);
    }
}