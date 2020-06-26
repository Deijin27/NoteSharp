using System.Collections.Generic;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Resources;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesMasterPageMaster : ContentPage
    {
 
        public NotesMasterPageMaster()
        {
            InitializeComponent();

            List<ListViewMenuItem> menuItems = new List<ListViewMenuItem>();
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf1c6", Text = AppResources.MasterOption_Settings, TargetType = typeof(SettingsPage)});
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf350", Text = AppResources.MasterOption_StyleSheets, TargetType = typeof(StyleSheetsPage)});
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf227", Text = AppResources.MasterOption_QuickAccess, TargetType = typeof(NotesPage)});

            menu.ItemsSource = menuItems;
        }
    }
}