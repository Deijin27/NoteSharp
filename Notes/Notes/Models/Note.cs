using System;
using SQLite;

namespace Notes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int FolderID { get; set; }
        public string Name { get; set; } // Make sure no note in the folder has the same name as it when setting.
        public string Text { get; set; }
        public bool IsQuickAccess { get; set; }
        public string FileType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        //public DateTime DateAccessed { get; set; }

        public DateTime DateCreatedLocal
        {
            get { return DateCreated.ToLocalTime(); }
        }

        public DateTime DateModifiedLocal
        {
            get { return DateModified.ToLocalTime(); }
        }

    }
}
