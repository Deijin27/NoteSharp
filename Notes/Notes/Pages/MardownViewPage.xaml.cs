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

namespace Notes.Pages
{
    public partial class MarkdownViewPage : ContentPage
    {


        string MarkdownText;

        public MarkdownViewPage(string markdownText, bool DisableCssSelector = false)
        {
            Console.WriteLine("DEBUG: MarkdownViewPage constructor called."); // DEBUG
            InitializeComponent();
            if (DisableCssSelector)
            {
                ToolbarItems.Remove(SelectCSS);
            }
            MarkdownText = markdownText;
            UpdateWebView();
            
        }

        async void UpdateWebView()
        {

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();

            string html = Markdown.ToHtml(MarkdownText, pipeline);

            //string css = @"h1 {
            //                   color: lightcoral;
            //                   font-weight: bold;
            //                   font-size: xx-large;
            //               }";

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