using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public interface IEntry
    {
        string Name { get; set; }
        string Text { get; set; }
    }
}
