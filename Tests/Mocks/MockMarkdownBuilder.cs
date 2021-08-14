using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Notes.Services;

namespace Tests.Mocks
{
    public class MockMarkdownBuilder : IMarkdownBuilder
    {
        public string BuildHtmlResult;
        public Queue<string> BuildHtmlCalls = new Queue<string>();
        public Task<string> BuildHtml(string text)
        {
            BuildHtmlCalls.Enqueue(text);
            return Task.FromResult(BuildHtmlResult);
        }

        public string BuildMarkdownResult;
        public bool BuildMarkdownErrorEncountered;
        public Queue<(string text, Guid folderId)> BuildMarkdownCalls = new Queue<(string text, Guid folderId)>();
        public Task<(string result, bool errorEncountered)> BuildMarkdown(string text, Guid folderId)
        {
            BuildMarkdownCalls.Enqueue((text, folderId));
            return Task.FromResult((BuildMarkdownResult, BuildMarkdownErrorEncountered));
        }
    }
}
