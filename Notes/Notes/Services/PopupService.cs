using Notes.PopupPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Notes.Models;
using System.Linq;

namespace Notes.Services
{
    public enum AlertPopupResult
    {
        Dismiss,
        Background,
        HardwareBack
    }

    public enum TwoOptionPopupResult
    {
        LeftButton,
        RightButton,
        Background,
        HardwareBack
    }

    public enum ListPopupResult
    {
        ListItem,
        Cancel,
        Background,
        HardwareBack
    }


    public class PopupService : IPopupService
    {
        public PopupService(IAppServiceProvider services)
        {
            Services = services;
        }
        IAppServiceProvider Services;

        public async Task PopAsync()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async Task PopAllAsync()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }

        private async Task PushPopup(Rg.Plugins.Popup.Pages.PopupPage page)
        {
            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task<AlertPopupResult> AlertPopup(string title, string message, string dismissButtonText)
        {
            var popup = new AlertPopupPage(title, message, dismissButtonText);
            await PushPopup(popup);
            return await popup.Result.Task;
        }

        public async Task<TwoOptionPopupResult> TwoOptionPopup(string title, string message, string leftButtonText,
            string rightButtonText)
        {
            var popup = new TwoOptionPopupPage(title, message, leftButtonText, rightButtonText);
            await PushPopup(popup);
            return await popup.Result.Task;
        }

        public async Task<(TwoOptionPopupResult Selected, string Text)> PromptPopup(string title, string message, string leftButtonText, string rightButtonText,
            string initialEntryText = "", string placeholderText = "")
        {
            var popup = new PromptPopupPage(title, message, leftButtonText, rightButtonText, initialEntryText, placeholderText);
            await PushPopup(popup);
            return await popup.Result.Task;

        }

        public async Task<(ListPopupResult Choice, TItem SelectedItem)> ListPopup<TItem>(string title, string message, string cancelButtonText, 
            IEnumerable<ListPopupPageItem<TItem>> items)
        {
            var popup = new ListPopupPage(title, message, cancelButtonText, 
                items.Select(i => new ListPopupPageItem() { Name = i.Name, AssociatedObject = i.AssociatedObject}));

            await PushPopup(popup);
            var (Choice, SelectedItem) = await popup.Result.Task;
            
            if (SelectedItem == null)
            {
                return await Task.FromResult((Choice, default(TItem)));
            }
            else
            {
                return await Task.FromResult((Choice, (TItem)SelectedItem));
            }
        }

        public async Task ColorPickerPopup()
        {
            var popup = new ColorPickerPopupPage();
            await PushPopup(popup);
        }

    }
}
