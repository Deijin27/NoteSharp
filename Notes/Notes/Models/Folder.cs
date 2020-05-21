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
    }
}
