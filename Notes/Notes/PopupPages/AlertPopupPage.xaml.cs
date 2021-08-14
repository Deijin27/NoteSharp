using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;
using Notes.Services;
using System.Threading.Tasks;

namespace Notes.PopupPages
{
    

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertPopupPage : PopupPage
    {
        public AlertPopupPage(string title, string message, string dismissButtonText)
        {
            InitializeComponent();

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            DismissButton.Text = dismissButtonText;
            Result = new TaskCompletionSource<AlertPopupResult>();
        }

        protected override bool OnBackgroundClicked()
        {
            ReturnValue = AlertPopupResult.Background;
            return base.OnBackgroundClicked();
        }

        protected override bool OnBackButtonPressed()
        {
            ReturnValue = AlertPopupResult.HardwareBack;
            return base.OnBackButtonPressed();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Result.SetResult(ReturnValue);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await AppServiceProvider.Instance.Popups.PopAsync();
        }

        AlertPopupResult ReturnValue;

        public TaskCompletionSource<AlertPopupResult> Result { get; }
    }
}