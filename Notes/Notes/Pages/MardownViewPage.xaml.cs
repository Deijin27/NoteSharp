using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;
using Markdig;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Notes.Data;

namespace Notes.Pages
{
    public partial class MarkdownViewPage : ContentPage
    {
        string MarkdownText;
        string HtmlText;

        public MarkdownViewPage(string markdownText, int folderID, bool DisableCssSelector = false)
        {
            InitializeComponent();
            if (DisableCssSelector)
            {
                ToolbarItems.Remove(SelectCSS);
            }
            MarkdownText = markdownText;
            Console.WriteLine("DEBUG: Initialise Html Started");
            InitialiseHtml(folderID);
            Console.WriteLine("DEBUG: Initialise Html Finished");
            Console.WriteLine("DEBUG: Update Web View Started");
            Console.WriteLine("DEBUG: Update Web View Finished");

        }

        async void InitialiseHtml(int folderID)
        {
            (string markdownFinal, ErrorEncountered errorEncountered) = await App.Database.InterpolateAndInputTemplatesAsync(MarkdownText, this, folderID);

            Console.WriteLine($"DEBUG: Interpolate crap finished, markdownfinal = [{markdownFinal}]");

            if (errorEncountered == ErrorEncountered.True)
            {
                Console.WriteLine("DEBUG: Error Encountered in InitialiseHtml");
                await Navigation.PopAsync();
            }
            else 
            {
                Console.WriteLine("DEBUG: Error NOT Encountered in InitialiseHtml");
                MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();
                HtmlText = Markdown.ToHtml(markdownFinal, pipeline);
                Console.WriteLine($"DEBUG: HtmlText set to [{HtmlText}]");

                UpdateWebView();
            }
        }

        async void UpdateWebView()
        {
            string html = HtmlText;

            string css = (await ((App)App.Current).GetStyleSheetAsync()).Text;

            html = "<style>\n" + css + "\n</style>\n" + html; // maybe add some <body> tags so that the user can affect the whole thing with styles.
            var htmlSource = new HtmlWebViewSource
            {
                Html = html
            };
            MarkdownWebView.Source = htmlSource;

        }

        async void SelectCSS_Clicked(object sender, EventArgs e)
        {
            List<CSS> sheetList = await App.GetAllStyleSheetsAsync();
            string[] sheetNameList = sheetList.Select(i => i.Name).ToArray();
            string sheetName = await DisplayActionSheet("Select CSS:", "Cancel", null, sheetNameList);
            if (sheetName != "Cancel")
            { 
                CSS sheet = sheetList.Where(i => i.Name == sheetName).FirstOrDefault();
                App.StyleSheetID = sheet.ID;
                UpdateWebView();
            }
        }

        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}