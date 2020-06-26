using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RadioCircle : RadioContentView
    {
        public RadioCircle()
        {
            InitializeComponent();
            CheckLabel.IsVisible = IsChecked;
        }

        public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create
        (
            nameof(BackgroundColor), 
            typeof(Color), 
            typeof(RadioCircle), 
            default(Color),
            BindingMode.OneWay
        );

        public new Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }

            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly BindableProperty CheckColorProperty = BindableProperty.Create
        (
            nameof(CheckColor),
            typeof(Color),
            typeof(RadioCircle),
            default(Color),
            BindingMode.OneWay
        );

        public Color CheckColor
        {
            get { return (Color)GetValue(CheckColorProperty); }

            set { SetValue(CheckColorProperty, value); }
        }

        public static readonly BindableProperty RadiusProperty = BindableProperty.Create
        (
            nameof(Radius),
            typeof(int),
            typeof(RadioCircle),
            default(int),
            BindingMode.OneWay
        );

        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }

            set { SetValue(RadiusProperty, value); }
        }

        public static readonly BindableProperty CheckSizeProperty = BindableProperty.Create
        (
            nameof(CheckSize),
            typeof(double),
            typeof(RadioCircle),
            default(double),
            BindingMode.OneWay
        );

        public double CheckSize
        {
            get { return (double)GetValue(CheckSizeProperty); }

            set { SetValue(CheckSizeProperty, value); }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BackgroundColorProperty.PropertyName)
            {
                BackgroundButton.BackgroundColor = BackgroundColor;
            }
            else if (propertyName == CheckColorProperty.PropertyName)
            {
                CheckLabel.TextColor = CheckColor;
            }
            else if (propertyName == RadiusProperty.PropertyName)
            {
                int radius = Radius;
                double diameter = radius * 2;
                BackgroundButton.HeightRequest = diameter;
                BackgroundButton.WidthRequest = diameter;
                BackgroundButton.CornerRadius = radius;
                ForegroundButton.HeightRequest = diameter;
                ForegroundButton.WidthRequest = diameter;
                ForegroundButton.CornerRadius = radius;
            }
            else if (propertyName == CheckSizeProperty.PropertyName)
            {
                CheckLabel.FontSize = CheckSize;
            }
            else if (propertyName == IsCheckedProperty.PropertyName)
            {

                CheckLabel.IsVisible = IsChecked;
            }
        }
    }
}