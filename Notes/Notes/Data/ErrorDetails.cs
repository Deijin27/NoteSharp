using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Data
{
    public class ErrorDetails
    {
        public bool ErrorWasEncountered { get; set; } = true;
        public string Title { get; set; }
        public string Message { get; set; }
        public string DismissButtonText { get; set; }
    }
}
