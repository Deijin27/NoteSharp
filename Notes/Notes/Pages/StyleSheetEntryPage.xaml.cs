using System;
using Xamarin.Forms;
using Notes.Models;
using Xamarin.Essentials;
using System.Threading;
using Amporis.Xamarin.Forms.ColorPicker;

namespace Notes.Pages
{
    public partial class StyleSheetEntryPage : ContentPage
    {
        string InitialName;
        string InitialText;

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

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var sheet = (CSS)BindingContext;

            await App.Database.SaveSheetAsync(sheet);
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
                bool answer = await DisplayAlert("Exit?", "Exit without saving changes?", "Yes", "No");
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

            await Navigation.PushAsync(new MarkdownViewPage(html, 0, true) { Title = "CSS Test View" });
        }

        private async void CopyButton_Clicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(TextEditor.Text);
            await DisplayAlert("Copied CSS", "Style text copied to clipboard", "OK");
        }

        private async void ColorPickerButton_Clicked(object sender, EventArgs e)
        {
            var colorDialogSettings = new ColorDialogSettings()
            {
                OkButtonText = "Copy",
                EditAlfa = false,
                DialogAnimation = false
                // More Settings Can Go Here
            };

            Color color = await ColorPickerDialog.Show(ColorPickerUtils.GetRootParent<Layout<View>>((View)TextEditor), 
                "Color Picker", Color.White, colorDialogSettings);

            string hexNoAlpha = color.ToHex().Remove(1, 2);

            await Clipboard.SetTextAsync(hexNoAlpha);
        }
    }
}