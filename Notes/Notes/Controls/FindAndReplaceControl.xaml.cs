using Notes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FindAndReplaceControl : Grid
    {
        public FindAndReplaceControl()
        {
            TextToFind = "";
            ReplaceWith = "";
            CaseSensitive = true;
            UseRegex = false;
            BindingContext = this;

            InitializeComponent();
        }

        public Editor BoundEditor { get; set; }
        public Note BoundNote { get; set; }

        public string TextToFind { get; set; }
        public string ReplaceWith { get; set; }
        public bool CaseSensitive { get; set; }
        public bool UseRegex { get; set; }

        public MatchCollection Matches { get; set; }
        public int MatchIndex { get; set; }

        private void Next_Clicked(object sender, EventArgs e)
        {
            //int count = Matches.Count;
            //if (count > 0)
            //{
            //    MatchIndex = (MatchIndex + 1) % count;
            //    Match match = Matches[MatchIndex];
            //    BoundEditor.CursorPosition = match.Index; // This isn't yet implemented for editor. My plan has been foiled!
            //}
        }

        private void Previous_Clicked(object sender, EventArgs e)
        {

        }

        private void Replace_Clicked(object sender, EventArgs e)
        {

        }

        private void ReplaceAll_Clicked(object sender, EventArgs e)
        {
            //BoundNote.Text = Regex.Replace(BoundNote.Text, TextToFind, ReplaceWith);
        }

        private void FindEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Matches = Regex.Matches(BoundNote.Text, TextToFind);
            //MatchIndex = -1;
            //Next_Clicked(null, null);
        }
    }
}