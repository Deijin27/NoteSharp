using System;
using SQLite;

namespace Notes.Models
{
    public class Folder
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
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
