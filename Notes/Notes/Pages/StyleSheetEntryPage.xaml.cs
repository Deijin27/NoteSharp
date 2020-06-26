using System;
using Xamarin.Forms;
using Notes.Models;
using Xamarin.Essentials;
using System.Threading;
using Amporis.Xamarin.Forms.ColorPicker;
using Notes.Resources;

namespace Notes.Pages
{
    public partial class StyleSheetEntryPage : ContentPage
    {
        string InitialName;
        string InitialText;
        bool NewSheet = false;

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

            BindingContext = sheet;
        }

        public StyleSheetEntryPage()
        {
            InitializeComponent();

            ToolbarItems.Remove(CopyButton);

            NewSheet = true;

            CSS sheet = new CSS() { IsReadOnly = false };

            InitialText = string.Copy(sheet.Text);
            InitialName = string.Copy(sheet.Name);

            BindingContext = sheet;
        }

        void UnfocusAll()
        {
            TextEditor.Unfocus();
            NameEntry.Unfocus();
        }

        public event ChangesSavedEventHandler ChangesSaved;

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var sheet = (CSS)BindingContext;
            sheet.DateModified = DateTime.UtcNow;
            if (NewSheet)
            {
                sheet.DateCreated = sheet.DateModified;
            }
            await App.Database.SaveSheetAsync(sheet);
            ChangesSaved?.Invoke();
            UnfocusAll();
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            CSS sheet = (CSS)BindingContext;

            if (sheet.IsReadOnly)
            {
                await Navigation.PopModalAsync();
            }
            else if (sheet.Name != InitialName || sheet.Text != InitialText)
            {
                bool answer = await DisplayAlert
                (
                    AppResources.Alert_ExitWithoutSaving_Title,
                    AppResources.Alert_ExitWithoutSaving_Message,
                    AppResources.AlertOption_Yes,
                    AppResources.AlertOption_No
                );
                if (answer)
                {
                    UnfocusAll();
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                UnfocusAll();
                await Navigation.PopModalAsync();
            }
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
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
            await DisplayAlert
            (
                AppResources.Alert_CSSCopied_Title, 
                AppResources.Alert_CSSCopied_Message, 
                AppResources.AlertOption_OK
            );
        }

        private async void ColorPickerButton_Clicked(object sender, EventArgs e)
        {
            var colorDialogSettings = new ColorDialogSettings()
            {
                OkButtonText = AppResources.ColorPicker_Copy_Button,
                EditAlfa = false,
                DialogAnimation = false
                // More Settings Can Go Here
            };

            Color color = await ColorPickerDialog.Show
            (
                ColorPickerUtils.GetRootParent<Layout<View>>((View)TextEditor), 
                AppResources.ColorPicker_Title, 
                Color.White, 
                colorDialogSettings
            );

            string hexNoAlpha = color.ToHex().Remove(1, 2);

            await Clipboard.SetTextAsync(hexNoAlpha);
        }
    }
}