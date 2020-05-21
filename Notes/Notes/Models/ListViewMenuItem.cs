using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public class ListViewMenuItem
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public Type TargetType { get; set; }
    }
}
