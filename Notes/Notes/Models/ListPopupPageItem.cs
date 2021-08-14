using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Models
{
    public class ListPopupPageItem
    {
        public string Name { get; set; }

        public object AssociatedObject { get; set; }
    }

    public class ListPopupPageItem<T>
    {
        public string Name { get; set; }

        public T AssociatedObject { get; set; }
    }
}
