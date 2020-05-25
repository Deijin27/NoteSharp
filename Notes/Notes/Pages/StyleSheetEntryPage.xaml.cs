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

        public StyleSheetEntryPage(bool isReadOnly=false, bool NewSheet = false)
        {
            InitializeComponent();

            if (isReadOnly)
            {
                // technically, if I disable the save button theres no need to make them readonly. 
                // But I'll leave it like this for now.
                TextEditor.IsReadOnly = true;
                NameEntry.IsReadOnly = true;
                ToolbarItems.Remove(SaveButton);
                ToolbarItems.Remove(DeleteButton);
                ToolbarItems.Remove(ColorPickerButton);
                Title = "Built In CSS";
            }
            else
            {
                ToolbarItems.Remove(CopyButton);
            }

            if (NewSheet)
            {
                ToolbarItems.Remove(DeleteButton);
            }
            
        }

        public string OriginalSheetName = null;

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var sheet = (CSS)BindingContext;
            if (sheet.Name == null) // null is the value when the text input is empty
            {
                sheet.Name = "Untitled Sheet";
            }
            // The sheet name is always changed from being equal to the default OriginalSheetName
            // So that if it's a new sheet, the name checks always occurr.
            if (sheet.Name != OriginalSheetName)
            {
                if (sheet.Name == "Cancel") // Ensure no interference with Cancel button of the dodgy picker dialog.
                {
                    sheet.Name = "Cancel_";
                }
                // Make sure no duplicate names because it would break MarkDownViewPage selector.
                string newName = sheet.Name;
                int count = 1;
                while ((await ((App)App.Current).DoesSheetNameExist(newName)))
                {
                    newName = $"{sheet.Name}[{count}]";
                    count++;
                }
                sheet.Name = newName;
            }
            await App.Database.SaveSheetAsync(sheet);
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var sheet = (CSS)BindingContext;
            await App.Database.DeleteSheetAsync(sheet);
            await Navigation.PopAsync();

            // If this sheet is set as the current sheet, then reset that to default.
            if (sheet.ID == App.StyleSheetID)
            {
                App.StyleSheetID = App.DefaultStyleSheetID;
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

            await Navigation.PushAsync(new MarkdownViewPage(html, true) { Title = "CSS Test View"});
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
                // More Settings Can Go Here
            };

            Color color = await ColorPickerDialog.Show(ColorPickerUtils.GetRootParent<Layout<View>>((View)NameEntry), 
                "Color Picker", Color.White, colorDialogSettings);

            string colorString = $"rgba({(int)(color.R * 255)}, {(int)(color.G * 255)}, {(int)(color.B * 255)}, {color.A:0.00})";

            await Clipboard.SetTextAsync(colorString);
        }
    }
}