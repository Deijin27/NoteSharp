using SQLite;
using System;

namespace Notes.Models
{
    class DeletedItem
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public DateTime DateDeleted { get; set; }
    }
}
