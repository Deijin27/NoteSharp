using System;
using System.IO;
using Xamarin.Forms;
using Notes.Models;
using System.Text.RegularExpressions;

namespace Notes.Pages
{
    public partial class HtmlPreviewPage : ContentPage
    {
        public HtmlPreviewPage(string htmlString)
        {
            Console.WriteLine("DEBUG: HtmlPreviewPage constructor called."); // DEBUG
            InitializeComponent();
            HtmlEditor.Text = htmlString;
        }
    }
}