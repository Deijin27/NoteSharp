using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Resources;

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
            mainNavigation = new NavigationPage(new NotesPage { Title = AppResources.PageTitle_RootFolder, FolderID = Guid.Empty });
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
                    await mainNavigation.PushAsync(new NotesPage(isQuickAccessPage: true) 
                    { 
                        Title = AppResources.PageTitle_QuickAccess
                    });
                }
                else if (item.TargetType == typeof(SettingsPage))
                {
                    var page = new SettingsPage();
                    page.RestoredBackupEvent += ((NotesPage)mainNavigation.RootPage).UpdateListView;

                    await mainNavigation.PushAsync(page);
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