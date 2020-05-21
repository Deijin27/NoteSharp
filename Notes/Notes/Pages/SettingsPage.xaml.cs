using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Themes;
using Xamarin.Essentials;

namespace Notes.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            
            InitializeComponent();
            if (App.GetTheme() == AppTheme.Dark)
            {
                ThemeSwitch.IsToggled = true;
            }
            else
            {
                ThemeSwitch.IsToggled = false;
            }
        }

        private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                App.SetTheme(AppTheme.Dark);
            }
            else
            {
                App.SetTheme(AppTheme.Light);
            }
        }
    }
}