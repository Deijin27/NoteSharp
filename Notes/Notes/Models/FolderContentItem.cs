using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    class FolderContentItem
    {
        public string Identifier { get; set; }
        public Folder ContentFolder { get; set; }
        public Note ContentNote { get; set; }
        public string Icon { get; set; }

        public FolderContentItem(Folder folder)
        {
            ContentFolder = folder;
            Identifier = "Folder";
        }

        public FolderContentItem(Note note)
        {
            ContentNote = note;
            Identifier = "File";
        }
    }
}
