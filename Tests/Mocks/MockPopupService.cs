using Notes.Models;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    public class MockPopupService : IPopupService
    {
        public AlertPopupResult AlertPopupResult;
        public int AlertPopupCallCount = 0;
        public Task<AlertPopupResult> AlertPopup(string title, string message, string dismissButtonText)
        {
            AlertPopupCallCount++;
            return Task.FromResult(AlertPopupResult);
        }

        public Task PopAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopAsync()
        {
            throw new NotImplementedException();
        }

        public (TwoOptionPopupResult Selected, string Text) PromptPopupResult;
        public int PromptPopupCallCount = 0;
        public Task<(TwoOptionPopupResult Selected, string Text)> PromptPopup(string title, string message, string leftButtonText, string rightButtonText, string initialEntryText = "", string placeholderText = "")
        {
            return Task.FromResult(PromptPopupResult);
        }

        public TwoOptionPopupResult TwoOptionPopupResult;
        public int TwoOptionPopupCallCount = 0;
        public Task<TwoOptionPopupResult> TwoOptionPopup(string title, string message, string leftButtonText, string rightButtonText)
        {
            TwoOptionPopupCallCount++;
            return Task.FromResult(TwoOptionPopupResult);
        }

        public int ListPopupCallCount = 0;
        public (ListPopupResult Choice, object SelectedItem) ListPopupResult;
        public Task<(ListPopupResult Choice, TItem SelectedItem)> ListPopup<TItem>(string title, string message, string cancelButtonText, IEnumerable<ListPopupPageItem<TItem>> items)
        {
            ListPopupCallCount++;
            return Task.FromResult((ListPopupResult.Choice, (TItem)ListPopupResult.SelectedItem));
        }

        public Task ColorPickerPopup()
        {
            throw new NotImplementedException();
        }
    }
}
