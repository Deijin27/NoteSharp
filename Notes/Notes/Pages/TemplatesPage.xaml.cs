using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Models;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TemplatesPage : NotesPage
    {
        public async new Task UpdateListView()
        {
            var fileItems = await App.Database.GetTemplatesAsync();

            var listViewItems = new List<FolderContentItem>();

            foreach (Note file in fileItems)
            {
                listViewItems.Add(new FolderContentItem(file));
            }

            //listView.ItemsSource = listViewItems;
        }
    }
}