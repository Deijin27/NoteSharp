using Notes.Services;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Notes.PopupPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoOptionPopupPage : PopupPage
    {
        public TwoOptionPopupPage(string title, string message, string leftButtonText, string rightButtonText)
        {
            InitializeComponent();

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            LeftButton.Text = leftButtonText;
            RightButton.Text = rightButtonText;
            Result = new TaskCompletionSource<TwoOptionPopupResult>();
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

        private async void RightButton_Clicked(object sender, EventArgs e)
        {
            ReturnValue = TwoOptionPopupResult.RightButton;
            await AppServiceProvider.Instance.Popups.PopAsync();
        }

        private async void LeftButton_Clicked(object sender, EventArgs e)
        {
            ReturnValue = TwoOptionPopupResult.LeftButton;
            await AppServiceProvider.Instance.Popups.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Result.SetResult(ReturnValue);
        }

        TwoOptionPopupResult ReturnValue;
        public TaskCompletionSource<TwoOptionPopupResult> Result { get; }
    }
}