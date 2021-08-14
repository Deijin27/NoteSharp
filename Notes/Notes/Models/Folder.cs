using System;
using SQLite;

namespace Notes.Models
{
    public class Folder : IChangeTracked
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public string Name { get; set; }
        public bool IsQuickAccess { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

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
