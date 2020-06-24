using System;

namespace Notes.Models
{
    interface IChangeTracked
    {
        Guid ID { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}
