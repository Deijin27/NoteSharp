using Notes.Models;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notes.ViewModels
{
    public class QuickAccessPageViewModel : FolderViewModelBase
    {
        public QuickAccessPageViewModel(IAppServiceProvider services) : base(services) { }
        public override void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            UpdateListView();
        }

        protected async override void UpdateListView()
        {
            SortingMode sortingMode = Services.Preferences.SortingMode;

            var items = new List<FolderContentItem>();

            var folders = await Services.NoteDatabase.GetQuickAccessFoldersAsync();
            var notes = await Services.NoteDatabase.GetQuickAccessNotesAsync();

            IEnumerable<FolderContentItem> contentFolders = folders
                .OrderBySortingMode(sortingMode)
                .Select(i => new FolderContentItem(i));
            IEnumerable<FolderContentItem> contentNotes = notes
                .OrderBySortingMode(sortingMode)
                .Select(i => new FolderContentItem(i));

            items.AddRange(contentFolders);
            items.AddRange(contentNotes);
            FolderContentItems = items;
        }
    }
}
