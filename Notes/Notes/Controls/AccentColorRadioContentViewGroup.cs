using Notes.AccentColors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notes.Controls
{
    class AccentColorRadioContentViewGroup : RadioContentViewGroup
    {
        public void Add(RadioContentView item, AppAccentColor associatedAccentColor)
        {
            item.AssociatedObject = associatedAccentColor;
            item.Clicked += SetAppAccentColor;
            base.Add(item);
        }

        public void SetAppAccentColor(object sender, EventArgs e)
        {
            RadioContentView item = (RadioContentView)sender;
            App.AccentColor = (AppAccentColor)item.AssociatedObject;
        }

        public void InitializeSelected(AppAccentColor accentColor)
        {
            SelectedID = this.Where(i => (AppAccentColor)i.AssociatedObject == accentColor).First().RadioGroupID;
        }
    }
}
