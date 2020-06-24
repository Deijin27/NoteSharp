using Notes.AccentColors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notes.Controls
{
    //class AccentColorRadioContentViewGroup : RadioContentViewGroup
    //{
    //    public void Add(AccentColorRadioContentView item, AppAccentColor associatedAccentColor)
    //    {
    //        item.AssociatedAccentColor = associatedAccentColor;
    //        item.Clicked += SetAppAccentColor;
    //        base.Add(item);
    //    }

    //    public void SetAppAccentColor(object sender, EventArgs e)
    //    {
    //        AccentColorRadioContentView item = (AccentColorRadioContentView)sender;
    //        App.AccentColor = item.AssociatedAccentColor;
    //    }

    //    public void InitializeSelected(AppAccentColor accentColor)
    //    {
    //        SelectedID = this.Where(i => (AppAccentColor)i.AssociatedAccentColor == accentColor).First().RadioGroupID;
    //    }
    //}

    //public interface IRadioAccentColorObject : IRadioObject
    //{
    //    AppAccentColor AssociatedAccentColor { get; set; }
    //}

    //class AccentColorRadioContentView : RadioContentView
    //{
    //    public AppAccentColor AssociatedAccentColor;
    //}

    class AccentColorRadioObjectGroup : RadioObjectGroup
    {
        public void Add(IRadioObject item, AppAccentColor associatedAccentColor)
        {
            item.AssociatedObject = associatedAccentColor;
            item.Clicked += SetAppAccentColor;
            base.Add(item);
        }

        public void SetAppAccentColor(object sender, EventArgs e)
        {
            IRadioObject item = (IRadioObject)sender;
            App.AccentColor = (AppAccentColor)item.AssociatedObject;
        }

        public void InitializeSelected(AppAccentColor accentColor)
        {
            SelectedID = this.Where(i => (AppAccentColor)i.AssociatedObject == accentColor).First().RadioGroupID;
        }
    }
}
