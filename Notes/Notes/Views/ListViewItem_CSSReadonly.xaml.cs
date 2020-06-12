using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewItem_CSSReadonly : ContentView
    {
        public ListViewItem_CSSReadonly()
        {
            InitializeComponent();
        }
    }
}