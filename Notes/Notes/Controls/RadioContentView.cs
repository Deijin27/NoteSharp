using System;
using System.Collections.Generic;
using System.Collections;
using Xamarin.Forms;

namespace Notes.Controls
{
    public delegate void IRadioObjectEventHandler(IRadioObject sender, EventArgs e);

    public interface IRadioObject
    {
        int RadioGroupID { get; set; }

        bool IsChecked { get; set; }

        event IRadioObjectEventHandler Clicked;

        object AssociatedObject { get; set; }
    }

    

    abstract public class RadioContentView : ContentView, IRadioObject // : CheckableContentView
    {
        public object AssociatedObject { get; set; }

        public int RadioGroupID { get; set; }

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

        public event IRadioObjectEventHandler Clicked;

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

    public interface IRadioObjectGroup : IEnumerable<IRadioObject>
    {
        int SelectedID { get; set; }

        void Add(IRadioObject item);

        void OnItemClicked(IRadioObject sender, EventArgs e);
    }

    public class RadioObjectGroup : IRadioObjectGroup // IEnumerable<RadioContentView>, 
    {
        private List<IRadioObject> internalList { get; set; } = new List<IRadioObject>();
        public IEnumerator<IRadioObject> GetEnumerator() => internalList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => internalList.GetEnumerator();

        // When inheriting, create a custom Add to be used by the constructor, add your own events and associated objects 
        // to the item then call base.Add(item) to finish.
        public virtual void Add(IRadioObject item)
        {
            item.Clicked += OnItemClicked;
            item.RadioGroupID = internalList.Count;
            internalList.Add(item);
        }

        public IRadioObject this[int index ]
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

        public void OnItemClicked(IRadioObject sender, EventArgs e)
        {
            SelectedID = sender.RadioGroupID;
        }
    }

    //public class RadioContentViewGroup : IRadioObjectGroup // IEnumerable<RadioContentView>, 
    //{
    //    private List<RadioContentView> internalList = new List<RadioContentView>();
    //    public IEnumerator<RadioContentView> GetEnumerator() => internalList.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => internalList.GetEnumerator();

    //    // When inheriting, create a custom Add to be used by the constructor, add your own events and associated objects 
    //    // to the item then call base.Add(item) to finish.
    //    public virtual void Add(RadioContentView item)
    //    {
    //        item.Clicked += OnItemClicked;
    //        item.RadioGroupID = internalList.Count;
    //        internalList.Add(item);
    //    }

    //    public RadioContentView this[int index]
    //    {
    //        get { return internalList[index]; }
    //    }

    //    private int selectedID;

    //    public int SelectedID
    //    {
    //        get
    //        {
    //            return selectedID;
    //        }
    //        set
    //        {
    //            internalList[selectedID].IsChecked = false;
    //            internalList[value].IsChecked = true;
    //            selectedID = value;
    //        }
    //    }

    //    public void OnItemClicked(object sender, EventArgs e)
    //    {
    //        RadioContentView item = (RadioContentView)sender;
    //        SelectedID = item.RadioGroupID;
    //    }
    //}
}
