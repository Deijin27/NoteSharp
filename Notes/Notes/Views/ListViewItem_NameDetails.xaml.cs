using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewItem_NameDetails : ContentView
    {
        public ListViewItem_NameDetails()
        {
            InitializeComponent();
            
        }

        public string IconGlyph // only ever connected to a static resource so no need for bindable property
        {
            get => IconLabel.Text;
            set => IconLabel.Text = value;
        }

        public static readonly BindableProperty IconColorProperty = BindableProperty.Create
        (
            nameof(IconColor),
            typeof(Color),
            typeof(ListViewItem_NameOnly),
            default(Color),
            BindingMode.OneWay
        );

        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public static readonly BindableProperty NameProperty = BindableProperty.Create
        (
            nameof(Name),
            typeof(string),
            typeof(ListViewItem_NameOnly),
            default(string),
            BindingMode.OneWay
        );

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly BindableProperty DetailsProperty = BindableProperty.Create
        (
            nameof(Details),
            typeof(string),
            typeof(ListViewItem_NameOnly),
            default(string),
            BindingMode.OneWay
        );

        public string Details
        {
            get => (string)GetValue(DetailsProperty);
            set => SetValue(DetailsProperty, value);
        }

        public static readonly BindableProperty IsQuickAccessProperty = BindableProperty.Create
        (
            nameof(IsQuickAccess),
            typeof(bool),
            typeof(ListViewItem_NameOnly),
            true, // important that this is true, since the default visibility is true
            BindingMode.OneWay
        );

        public bool IsQuickAccess
        {
            get => (bool)GetValue(IsQuickAccessProperty);
            set => SetValue(IsQuickAccessProperty, value);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Name):
                    NameLabel.Text = Name;
                    break;
                case nameof(IconColor):
                    IconLabel.TextColor = IconColor;
                    break;
                case nameof(Details):
                    DetailsLabel.Text = Details;
                    break;
                case nameof(IsQuickAccess):
                    QuickAccessMarkerLabel.IsVisible = IsQuickAccess;
                    break;
            };
        }
    }
}