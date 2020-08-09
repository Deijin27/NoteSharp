using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.PopupPages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UniqueNamePromptPopupPage : PopupPage
    {
        public delegate void OptionClicked(PromptPopupOptionEventArgs e);
        public delegate Task<bool> DoesNameExist(string name, Guid folderID, Guid id);

        #region Constructor

        public UniqueNamePromptPopupPage
        (
            string title,
            string message,
            string acceptButtonText,
            string cancelButtonText,
            Guid folderID,
            Guid id,
            DoesNameExist doesNameExistChecker,
            string nameInvalidWarningMessage,
            string nameConflictWarningMessage,
            string initialEntryText = "", 
            string placeholderText = ""
        )
        {
            InitializeComponent();

            NameConflictWarningMessage = nameConflictWarningMessage;
            NameInvalidWarningMessage = nameInvalidWarningMessage;
            
            FolderID = folderID;
            ID = id;
            DoesNameExistChecker = doesNameExistChecker;

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            AcceptButton.Text = acceptButtonText;
            CancelButton.Text = cancelButtonText;

            // these must be set last so that warning message is updated properly
            InputEntry.Text = initialEntryText;
            InputEntry.Placeholder = placeholderText;
        }

        #endregion

        private readonly string NameInvalidWarningMessage;
        private readonly string NameConflictWarningMessage;
        private readonly Guid FolderID;
        private readonly Guid ID;
        private readonly DoesNameExist DoesNameExistChecker;

        #region Pointless Bindable Properties
        /*
        public static new readonly BindableProperty TitleProperty = BindableProperty.Create
        (
            nameof(Title),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }

            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.Create
        (
            nameof(Message),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }

            set { SetValue(MessageProperty, value); }
        }

        public static readonly BindableProperty AcceptButtonTextProperty = BindableProperty.Create
        (
            nameof(AcceptButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string AcceptButtonText
        {
            get { return (string)GetValue(AcceptButtonTextProperty); }

            set { SetValue(AcceptButtonTextProperty, value); }
        }

        public static readonly BindableProperty CancelButtonTextProperty = BindableProperty.Create
        (
            nameof(CancelButtonText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }

            set { SetValue(CancelButtonTextProperty, value); }
        }

        public static readonly BindableProperty EntryTextProperty = BindableProperty.Create
        (
            nameof(EntryText),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string EntryText
        {
            get { return (string)GetValue(EntryTextProperty); }

            set { SetValue(EntryTextProperty, value); }
        }

        public static readonly BindableProperty EntryPlaceholderProperty = BindableProperty.Create
        (
            nameof(EntryPlaceholder),
            typeof(string),
            typeof(PromptPopupPage),
            default(string),
            BindingMode.OneWay
        );

        public string EntryPlaceholder
        {
            get { return (string)GetValue(EntryPlaceholderProperty); }

            set { SetValue(EntryPlaceholderProperty, value); }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == TitleProperty.PropertyName)
            {
                TitleLabel.Text = Title;
            }
            else if (propertyName == MessageProperty.PropertyName)
            {
                MessageLabel.Text = Message;
            }
            else if (propertyName == EntryTextProperty.PropertyName)
            {
                InputEntry.Text = EntryText;
            }
            else if (propertyName == AcceptButtonTextProperty.PropertyName)
            {
                AcceptButton.Text = AcceptButtonText;
            }
            else if (propertyName == CancelButtonTextProperty.PropertyName)
            {
                CancelButton.Text = CancelButtonText;
            }
            else if (propertyName == EntryPlaceholderProperty.PropertyName)
            {
                InputEntry.Placeholder = EntryPlaceholder;
            }
        }
        */
        #endregion

        #region Animation Stuff

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }
        #endregion
        
        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(InputEntry.Text);
            HardwareBackClicked?.Invoke(argsOut);
            // Return true if you don't want to close this popup page when a back button is pressed
            return true; //base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(InputEntry.Text);
            BackgroundClicked?.Invoke(argsOut);
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false; //base.OnBackgroundClicked();
        }

        public event OptionClicked CancelOptionClicked;
        public event OptionClicked AcceptOptionClicked;
        public new event OptionClicked BackgroundClicked;
        public event OptionClicked HardwareBackClicked;

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(InputEntry.Text);
            CancelOptionClicked?.Invoke(argsOut);
        }

        private void AcceptButton_Clicked(object sender, EventArgs e)
        {
            if (AcceptButtonEnabled)
            {
                PromptPopupOptionEventArgs argsOut = new PromptPopupOptionEventArgs(InputEntry.Text);
                AcceptOptionClicked?.Invoke(argsOut);
            }
        }

        private bool _acceptButtonEnabled = true;

        private bool AcceptButtonEnabled // pseudo enable disable so text doesnt go black when disabled
        {
            get
            {
                return _acceptButtonEnabled;
            }
            set
            {
                if (value == false)
                {
                    AcceptButton.Opacity = 0.5;
                    _acceptButtonEnabled = false;
                }
                else
                {
                    AcceptButton.Opacity = 1.0;
                    _acceptButtonEnabled = true;
                }
            }
        }

        private async void InputEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = e.NewTextValue;

            if (IsNameInvalid(name))
            {
                WarningLabel.Text = NameInvalidWarningMessage;
                AcceptButtonEnabled = false;
            }
            else if (await DoesNameExistChecker(e.NewTextValue, FolderID, ID))
            {
                WarningLabel.Text = NameConflictWarningMessage;
                AcceptButtonEnabled = false;
            }
            else
            {
                WarningLabel.Text = "";
                AcceptButtonEnabled = true;
            }
        }

        public static bool IsNameInvalid(string name)
        {
            return name.Contains("/") || name.Contains("\"") || name.StartsWith("*") || name.StartsWith("~") || name == "." || name == "..";
        }
    }
}