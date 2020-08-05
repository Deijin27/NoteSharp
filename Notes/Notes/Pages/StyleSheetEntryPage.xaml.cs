﻿using System;
using Xamarin.Forms;
using Notes.Models;
using Xamarin.Essentials;
using System.Threading;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;
using System.Threading.Tasks;

namespace Notes.Pages
{
    public partial class StyleSheetEntryPage : ContentPage
    {
        string InitialName;
        string InitialText;
        bool NewSheet = false;
        public CSS CurrentSheet { get; set; }
        public bool UnsavedChangesExist { get; set; }

        public StyleSheetEntryPage(CSS sheet)
        {
            InitializeComponent();

            if (sheet.IsReadOnly)
            {
                // technically, if I disable the save button theres no need to make them readonly. 
                // But I'll leave it like this for now.
                TextEditor.IsReadOnly = true;
                NameEntry.IsReadOnly = true;
                ToolbarItems.Remove(SaveButton);
                ToolbarItems.Remove(ColorPickerButton);
                Title = "Built In CSS";
            }
            else
            {
                ToolbarItems.Remove(CopyButton);
                InitialText = string.Copy(sheet.Text);
                InitialName = string.Copy(sheet.Name);
            }

            CurrentSheet = sheet;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        public StyleSheetEntryPage()
        {
            InitializeComponent();

            ToolbarItems.Remove(CopyButton);

            NewSheet = true;

            CSS sheet = new CSS() { IsReadOnly = false };

            InitialText = string.Copy(sheet.Text);
            InitialName = string.Copy(sheet.Name);

            CurrentSheet = sheet;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        void UnfocusAll()
        {
            TextEditor.Unfocus();
            NameEntry.Unfocus();
        }

        public event ChangesSavedEventHandler ChangesSaved;

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (UnsavedChangesExist)
                await Save();
        }

        async void Close_Clicked(object sender, EventArgs e)
        {
            if (CurrentSheet.IsReadOnly)
            {
                await Navigation.PopModalAsync();
            }
            else if (UnsavedChangesExist)
            {
                var popup = new TwoOptionPopupPage
                (
                    "Save Changes?",
                    "Unsaved changes exist, would you like to save them?",
                    "Yes",
                    "No"
                );
                popup.BackgroundClicked += CancelClose;
                popup.HardwareBackClicked += CancelClose;
                popup.LeftOptionClicked += SaveThenClose;
                popup.RightOptionClicked += CloseWithoutSaving;
                await PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                UnfocusAll();
                await Navigation.PopModalAsync();
            }
        }

        async void CancelClose() { await PopupNavigation.Instance.PopAsync(); }

        async void CloseWithoutSaving()
        {
            CurrentSheet.Name = InitialName;
            CurrentSheet.Text = InitialText;
            UnfocusAll();
            await PopupNavigation.Instance.PopAsync();
            await Navigation.PopModalAsync();
        }

        async void SaveThenClose()
        {
            await PopupNavigation.Instance.PopAsync();
            await Save();
            UnfocusAll();
            await Navigation.PopModalAsync();
        }

        async Task Save()
        {
            CurrentSheet.DateModified = DateTime.UtcNow;
            if (NewSheet)
            {
                CurrentSheet.DateCreated = CurrentSheet.DateModified;
            }
            await App.Database.SaveSheetAsync(CurrentSheet);
            ChangesSaved?.Invoke();
            UnsavedChangesExist = false;
            return;
        }
        

        async void QuickTest_Clicked(object sender, EventArgs e)
        {
            var sheet = (CSS)BindingContext;
            string css = sheet.Text;

            string html = "<h1>Test Heading</h1><p>Hello I am a testing robot bleep bloop</p>";

            html = "<style>\n" + css + "\n</style>\n" + html;

            await Navigation.PushAsync(new MarkdownViewPage(html, Guid.Empty, true) { Title = "CSS Test View" });
        }

        private async void CopyButton_Clicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(TextEditor.Text);

            // maybe a popup here that disappears on it's own
        }

        private async void ColorPickerButton_Clicked(object sender, EventArgs e)
        {
            var popup = new ColorPickerPopupPage
            (
                "Color Picker",
                "Note that to use transparency you need to use the Copy RGBA option.",
                "Cancel",
                "Copy Hex",
                "Copy RGBA",
                Color.IndianRed
            );

            await PopupNavigation.Instance.PushAsync(popup);
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }
    }
}