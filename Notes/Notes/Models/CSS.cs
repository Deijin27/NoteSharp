using System;
using System.Text.Json.Serialization;
using SQLite;

namespace Notes.Models
{
    public class CSS : IChangeTracked
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Name { get; set; } = ""; // Make sure no css has the same name as it when setting.
        public string Text { get; set; } = "";
        public bool IsReadOnly { get; set; }

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
