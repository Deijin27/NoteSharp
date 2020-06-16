using System;
using SQLite;
using System.Text.Json.Serialization;

namespace Notes.Models
{
    public class Folder
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public Guid ParentID { get; set; }
        public string Name { get; set; }
        public bool IsQuickAccess { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        [JsonIgnore]
        public DateTime DateCreatedLocal
        {
            get { return DateCreated.ToLocalTime(); }
        }

        [JsonIgnore]
        public DateTime DateModifiedLocal
        {
            get { return DateModified.ToLocalTime(); }
        }
    }
}
