using System;
using System.Collections.Generic;
using System.Collections;
using Xamarin.Forms;

namespace Notes.Controls
{
    public class RadioContentView : ContentView // : CheckableContentView
    {
        public int RadioGroupID;

        public object AssociatedObject; // store a reference to some associated object for use in custom inherior of RadioContentViewGroup

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create
        (
            nameof(IsChecked),
            typeof(bool),
            typeof(RadioContentView),
            default(bool),
            BindingMode.TwoWay
        );

        public bool IsChecked
        {
            get
            {
                return (bool)GetValue(IsCheckedProperty);
            }

            set
            {
                SetValue(IsCheckedProperty, value);
            }
        }

        public event EventHandler Clicked;

        /// <summary>
        /// Add to the Clicked EventHandler for the sub component to pass that event up to this component
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SendClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RadioContentViewGroup : IEnumerable<RadioContentView>
    {
        private List<RadioContentView> internalList = new List<RadioContentView>();
        public IEnumerator<RadioContentView> GetEnumerator() => internalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => internalList.GetEnumerator();

        // When inheriting, create a custom Add to be used by the constructor, add your own events and associated objects 
        // to the item then call base.Add(item) to finish.
        public virtual void Add(RadioContentView item)
        {
            item.Clicked += OnItemClicked;
            item.RadioGroupID = internalList.Count;
            internalList.Add(item);
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
