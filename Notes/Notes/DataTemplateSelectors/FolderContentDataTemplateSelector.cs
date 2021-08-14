using Notes.Models;
using Xamarin.Forms;
using Notes.Services;
using System;

namespace Notes.DataTemplateSelectors
{
    public class FolderContentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate_NameOnly { get; set; }
        public DataTemplate FolderTemplate_NameDateModified { get; set; }
        public DataTemplate FolderTemplate_NameDateCreated { get; set; }
        public DataTemplate FolderTemplate_NameSize { get; set; }
        public DataTemplate FileTemplate_NameOnly { get; set; }
        public DataTemplate FileTemplate_NameDateModified { get; set; }
        public DataTemplate FileTemplate_NameDateCreated { get; set; }
        public DataTemplate FileTemplate_NameSize { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var contentItem = (FolderContentItem)item;

            SortingMode sortingMode = AppServiceProvider.Instance.Preferences.SortingMode;

            if (contentItem.Identifier == FolderContentItemIdentifier.Folder)
            {
                return sortingMode switch
                {
                    SortingMode.Name => FolderTemplate_NameOnly,
                    SortingMode.DateCreated => FolderTemplate_NameDateCreated,
                    SortingMode.DateModified => FolderTemplate_NameDateModified,
                    SortingMode.Size => FolderTemplate_NameSize,
                    _ => FolderTemplate_NameOnly,
                };
            }

            return sortingMode switch // Note
            {
                SortingMode.Name => FileTemplate_NameOnly,
                SortingMode.DateCreated => FileTemplate_NameDateCreated,
                SortingMode.DateModified => FileTemplate_NameDateModified,
                SortingMode.Size => FileTemplate_NameSize,
                _ => FileTemplate_NameOnly,
            };
        }
    }
}
