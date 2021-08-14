using System;
using SQLite;

namespace Notes.Models
{
    public class CSS : IEntry
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public string Name { get; set; } = ""; // Make sure no css has the same name as it when setting.
        public string Text { get; set; } = "";
        public bool IsReadOnly { get; set; }
    }
}
