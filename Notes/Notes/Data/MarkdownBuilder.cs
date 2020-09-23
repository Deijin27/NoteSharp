using Markdig;
using System;
using System.Threading.Tasks;

namespace Notes.Data
{
    static class MarkdownBuilder
    {
        public static Task<(string result, bool errorEncountered)> BuildMarkdown(string text, Guid folderID)
        {
            return App.Database.InterpolateAndInputTemplatesAsync(text, folderID);
        }

        public static string BuildHtml(string text)
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();
            return Markdown.ToHtml(text, pipeline);
        }
    }
}
