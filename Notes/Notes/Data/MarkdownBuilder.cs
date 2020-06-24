using Markdig;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notes.Data
{
    static class MarkdownBuilder
    {
        public static Task<(string, ErrorEncountered)> BuildMarkdown(string text, Page page, Guid folderID)
        {
            return App.Database.InterpolateAndInputTemplatesAsync(text, page, folderID);
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
