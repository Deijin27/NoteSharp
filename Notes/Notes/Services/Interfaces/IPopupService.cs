using Notes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Services
{
    public interface IPopupService
    {
        Task<AlertPopupResult> AlertPopup(string title, string message, string dismissButtonText);
        Task ColorPickerPopup();
        Task<(ListPopupResult Choice, TItem SelectedItem)> ListPopup<TItem>(string title, string message, string cancelButtonText, IEnumerable<ListPopupPageItem<TItem>> items);
        Task PopAllAsync();
        Task PopAsync();
        Task<(TwoOptionPopupResult Selected, string Text)> PromptPopup(string title, string message, string leftButtonText, string rightButtonText, string initialEntryText = "", string placeholderText = "");
        Task<TwoOptionPopupResult> TwoOptionPopup(string title, string message, string leftButtonText, string rightButtonText);
    }
}