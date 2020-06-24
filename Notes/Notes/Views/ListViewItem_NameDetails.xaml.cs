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
            QuickAccessMarkerLabel.IsVisible = IsQuickAccess; // It's important not to forget to do this
        }

        public string IconGlyph // only ever connected to a static resource so no need for bindable property
        {
            get
            {
                return IconLabel.Text;
            }
            set
            {
                IconLabel.Text = value;
            }
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
            get
            {
                return (Color)GetValue(IconColorProperty);
            }
            set
            {
                SetValue(IconColorProperty, value);
            }
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
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
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
            get
            {
                return (string)GetValue(DetailsProperty);
            }
            set
            {
                SetValue(DetailsProperty, value);
            }
        }

        public static readonly BindableProperty IsQuickAccessProperty = BindableProperty.Create
        (
            nameof(IsQuickAccess),
            typeof(bool),
            typeof(ListViewItem_NameOnly),
            default(bool),
            BindingMode.OneWay
        );

        public bool IsQuickAccess
        {
            get
            {
                return (bool)GetValue(IsQuickAccessProperty);
            }
            set
            {
                SetValue(IsQuickAccessProperty, value);
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == NameProperty.PropertyName)
            {
                NameLabel.Text = Name;
            }
            else if (propertyName == IsQuickAccessProperty.PropertyName)
            {
                QuickAccessMarkerLabel.IsVisible = IsQuickAccess;
            }
            else if (propertyName == IconColorProperty.PropertyName)
            {
                IconLabel.TextColor = IconColor;
            }
            else if (propertyName == DetailsProperty.PropertyName)
            {
                DetailsLabel.Text = Details;
            }
        }
    }
}