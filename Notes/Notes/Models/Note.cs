using System;
using SQLite;

namespace Notes.Models
{
    public class Note : IChangeTracked, IEntry
    {
        public static Note New(Guid folderId)
        {
            return new Note()
            {
                ID = Guid.NewGuid(),
                FolderID = folderId,
                DateCreated = DateTime.UtcNow
            };
        }

        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid FolderID { get; set; }
        public string Name { get; set; } = ""; // Make sure no note in the folder has the same name as it when setting.
        public string Text { get; set; } = "";
        public bool IsQuickAccess { get; set; }
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

        public string ReadableSize => $"Size: {Text.ByteCount().ToReadableByteCountString()}";

    }
}
