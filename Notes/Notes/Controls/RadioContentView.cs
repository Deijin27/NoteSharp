using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Notes.Controls
{
    public class RadioContentView : CheckableContentView
    {
        public int RadioGroupID;

        public void SubComponent_Clicked(object sender, EventArgs e)
        {
            SendClicked();
        }
    }

    public class RadioContentViewGroup : IEnumerable<RadioContentView>
    {
        private List<RadioContentView> internalList = new List<RadioContentView>();
        public IEnumerator<RadioContentView> GetEnumerator() => internalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => internalList.GetEnumerator();

        public virtual void Add(RadioContentView item)
        {
            item.Clicked += OnItemClicked;
            item.RadioGroupID = internalList.Count;
            internalList.Add(item);
        }

        public void AddRange(IEnumerable<RadioContentView> collection)
        {
            foreach (RadioContentView item in collection)
            {
                Add(item);
            }
        }

        public RadioContentView this[int index ]
        {
            get { return internalList[index]; }
        }

        private int selectedID;

        public int SelectedID
        {
            get
            {
                return selectedID;
            }
            set
            {
                internalList[selectedID].IsChecked = false;
                internalList[value].IsChecked = true;
                selectedID = value;
            }
        }

        public void OnItemClicked(object sender, EventArgs e)
        {
            RadioContentView item = (RadioContentView)sender;
            SelectedID = item.RadioGroupID;
        }
    }
}
