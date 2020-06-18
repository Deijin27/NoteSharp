using Notes.AccentColors;
using System;
using System.Linq;

namespace Notes.Controls
{
    class AccentColorRadioContentViewGroup : RadioContentViewGroup
    {
        public override void Add(RadioContentView item)
        {
            item.Clicked += SetAppAccentColor;
            base.Add(item);
        }

        public void SetAppAccentColor(object sender, EventArgs e)
        {
            RadioContentView item = (RadioContentView)sender;
            App.AccentColor = (AppAccentColor)item.AssociatedEnum;
        }
    }
}
