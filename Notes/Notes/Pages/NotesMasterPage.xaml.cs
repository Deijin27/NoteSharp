using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesMasterPage : MasterDetailPage
    {

        NotesMasterPageMaster masterPage;
        NavigationPage mainNavigation;

        public NotesMasterPage()
        {
            InitializeComponent();
            mainNavigation = new NavigationPage(new NotesPage { Title = "Root", FolderID = Guid.Empty });
            Detail = mainNavigation;

            masterPage = new NotesMasterPageMaster();
            Master = masterPage;
            masterPage.menu.ItemSelected += OnItemSelected;

        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ListViewMenuItem;
            if (item != null)
            {
                if (item.TargetType == typeof(NotesPage))
                {
                    await mainNavigation.PushAsync(new NotesPage() { Title = "Quick Access", IsQuickAccessPage = true });
                }
                else
                { 
                    await mainNavigation.PushAsync((ContentPage)Activator.CreateInstance(item.TargetType));
                }
                masterPage.menu.SelectedItem = null;
                IsPresented = false;
            }
        }
    }

    
}