using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Notes.Services;

namespace Tests.Mocks
{
    class MockClipboardService : IClipboardService
    {
        public string GetTextResult;
        public int GetTextCallCount = 0;
        public Task<string> GetTextAsync()
        {
            GetTextCallCount++;
            return Task.FromResult(GetTextResult);
        }

        public Queue<string> SetTextCalls = new Queue<string>();
        public Task SetTextAsync(string text)
        {
            SetTextCalls.Enqueue(text);
            return Task.CompletedTask;
        }
    }
}
