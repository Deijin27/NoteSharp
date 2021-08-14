using Notes.Services;
using Notes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        //private AccentColorRadioObjectGroup accentRadioGroup;

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsPageViewModel(AppServiceProvider.Instance);

            //accentRadioGroup = new AccentColorRadioObjectGroup()
            //{
            //    { RedAccentColorRadioCircle, AppAccentColor.Red },
            //    { PinkAccentColorRadioCircle, AppAccentColor.Pink },
            //    { PurpleAccentColorRadioCircle, AppAccentColor.Purple },
            //    { DeepPurpleAccentColorRadioCircle, AppAccentColor.DeepPurple },
            //    { IndigoAccentColorRadioCircle, AppAccentColor.Indigo },
            //    { BlueAccentColorRadioCircle, AppAccentColor.Blue },
            //    { LightBlueAccentColorRadioCircle, AppAccentColor.LightBlue },
            //    { CyanAccentColorRadioCircle, AppAccentColor.Cyan },
            //    { TealAccentColorRadioCircle, AppAccentColor.Teal },
            //    { GreenAccentColorRadioCircle, AppAccentColor.Green },
            //    { LightGreenAccentColorRadioCircle, AppAccentColor.LightGreen },
            //    { LimeAccentColorRadioCircle, AppAccentColor.Lime },
            //    { YellowAccentColorRadioCircle, AppAccentColor.Yellow },
            //    { AmberAccentColorRadioCircle, AppAccentColor.Amber },
            //    { OrangeAccentColorRadioCircle, AppAccentColor.Orange },
            //    { DeepOrangeAccentColorRadioCircle, AppAccentColor.DeepOrange }
            //};
            //accentRadioGroup.InitializeSelected(Services.Theme.AccentColor);
        }

    }
}