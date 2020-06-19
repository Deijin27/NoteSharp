using System;
using Xamarin.Forms;

namespace Notes.Controls
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomSearchBar : StackLayout
    {
        public CustomSearchBar()
        {
            InitializeComponent();
        }

        #region SearchIconColor

        public static readonly BindableProperty SearchIconColorProperty = BindableProperty.Create
        (
            nameof(SearchIconColor),
            typeof(Color),
            typeof(CustomSearchBar),
            default(Color),
            BindingMode.OneWay
        );

        public Color SearchIconColor
        {
            get { return (Color)GetValue(SearchIconColorProperty); }

            set { SetValue(SearchIconColorProperty, value); }
        }

        #endregion

        #region CloseIconColor

        public static readonly BindableProperty CloseIconColorProperty = BindableProperty.Create
        (
            nameof(CloseIconColor),
            typeof(Color),
            typeof(CustomSearchBar),
            default(Color),
            BindingMode.OneWay
        );

        public Color CloseIconColor
        {
            get { return (Color)GetValue(CloseIconColorProperty); }

            set { SetValue(CloseIconColorProperty, value); }
        }

        #endregion

        #region TextColor

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create
        (
            nameof(TextColor),
            typeof(Color),
            typeof(CustomSearchBar),
            default(Color),
            BindingMode.OneWay
        );

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }

            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        #region PlaceholderColor

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create
        (
            nameof(PlaceholderColor),
            typeof(Color),
            typeof(CustomSearchBar),
            default(Color),
            BindingMode.OneWay
        );

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }

            set { SetValue(PlaceholderColorProperty, value); }
        }

        #endregion

        #region Text

        public static readonly BindableProperty TextProperty = BindableProperty.Create
        (
            nameof(Text),
            typeof(string),
            typeof(CustomSearchBar),
            default(string),
            BindingMode.TwoWay
        );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Placeholder

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create
        (
            nameof(Placeholder),
            typeof(string),
            typeof(CustomSearchBar),
            default(string),
            BindingMode.OneWay
        );

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }

            set { SetValue(PlaceholderProperty, value); }
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == SearchIconColorProperty.PropertyName)
            {
                searchIcon.TextColor = SearchIconColor;
            }
            else if (propertyName == CloseIconColorProperty.PropertyName)
            {
                closeButton.TextColor = CloseIconColor;
            }
            else if (propertyName == TextColorProperty.PropertyName)
            {
                searchInput.TextColor = TextColor;
            }
            else if (propertyName == PlaceholderColorProperty.PropertyName)
            {
                searchInput.PlaceholderColor = PlaceholderColor;
            }
            else if (propertyName == TextProperty.PropertyName)
            {
                searchInput.Text = Text;
            }
            else if (propertyName == PlaceholderProperty.PropertyName)
            {
                searchInput.Placeholder = Placeholder;
            }
        }

        public event EventHandler<TextChangedEventArgs> TextChanged;

        public void SendTextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        public event EventHandler CloseClicked;

        public void SendCloseClicked(object sender, EventArgs e)
        {
            CloseClicked?.Invoke(this, e);
        }

        private void searchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = e.NewTextValue;
            SendTextChanged(this, e);
        }

        public new bool Focus()
        {
            return searchInput.Focus();
        }
    }
}