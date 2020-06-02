using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public enum FolderContentItemIdentifier
    {
        Folder,
        File,
        Dataset
    }

    class FolderContentItem
    {
        public FolderContentItemIdentifier Identifier { get; set; }
        public Folder ContentFolder { get; set; }
        public Note ContentNote { get; set; }
        public Dataset ContentDataset { get; set; }
        public string Icon { get; set; }

        public FolderContentItem(Folder folder)
        {
            ContentFolder = folder;
            Identifier = FolderContentItemIdentifier.Folder;
        }

        public FolderContentItem(Note note)
        {
            ContentNote = note;
            Identifier = FolderContentItemIdentifier.File;
        }

        public FolderContentItem(Dataset dataset)
        {
            ContentDataset = dataset;
            Identifier = FolderContentItemIdentifier.Dataset;
        }
    }
}
