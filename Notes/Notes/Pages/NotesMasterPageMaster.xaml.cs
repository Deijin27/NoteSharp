using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesMasterPageMaster : ContentPage
    {
 
        public NotesMasterPageMaster()
        {
            InitializeComponent();

            List<ListViewMenuItem> menuItems = new List<ListViewMenuItem>();
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf1c6", Text = "Settings", TargetType = typeof(SettingsPage)});
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf350", Text = "Style Sheets", TargetType = typeof(StyleSheetsPage)});
            menuItems.Add(new ListViewMenuItem() { Icon = "\uf227", Text = "Quick Access", TargetType = typeof(NotesPage)});

            menu.ItemsSource = menuItems;
        }
    }
}