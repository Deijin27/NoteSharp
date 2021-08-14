using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using Notes.Services;

namespace Notes.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromptPopupPage : PopupPage
    {
        public PromptPopupPage(string title, string message, string leftButtonText, string rightButtonText, 
            string initialEntryText = "", string placeholderText = "")
        {
            InitializeComponent();

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            AcceptButton.Text = leftButtonText;
            CancelButton.Text = rightButtonText;
            InputEntry.Text = initialEntryText;
            InputEntry.Placeholder = placeholderText;
            Result = new TaskCompletionSource<(TwoOptionPopupResult Selected, string Text)>();
        }

        protected override bool OnBackButtonPressed()
        {
            ReturnValue = TwoOptionPopupResult.HardwareBack;
            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            ReturnValue = TwoOptionPopupResult.Background;
            return base.OnBackgroundClicked();
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            ReturnValue = TwoOptionPopupResult.RightButton;
            await AppServiceProvider.Instance.Popups.PopAsync();
        }

        private async void AcceptButton_Clicked(object sender, EventArgs e)
        {
            ReturnValue = TwoOptionPopupResult.LeftButton;
            await AppServiceProvider.Instance.Popups.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Result.SetResult((ReturnValue, InputEntry.Text));
        }

        TwoOptionPopupResult ReturnValue;

        public TaskCompletionSource<(TwoOptionPopupResult Selected, string Text)> Result { get; }
    }
}