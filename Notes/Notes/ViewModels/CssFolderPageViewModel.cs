using Notes.Models;
using Notes.Resources;
using Notes.RouteUtil;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public class CssFolderPageViewModel : CssListViewModelBase
    {
        public CssFolderPageViewModel(IAppServiceProvider services) : base(services) 
        {
            AddCssCommand = new Command(AddCss);

            DeleteCssCommand = new Command(DeleteCss);
            RenameCssCommand = new Command(RenameCss);
        }

        protected async override void OnListViewItemSelected(CSS value)
        {
            await Services.Navigation.GoToAsync<CssPageViewModel, CssPageSetupParameters>(
                new CssPageSetupParameters(value.ID));
        }

        public ICommand DeleteCssCommand { get; set; }
        public ICommand AddCssCommand { get; set; }
        public ICommand RenameCssCommand { get; set; }

        async void AddCss()
        {
            await Services.Navigation.GoToAsync<CssPageViewModel, CssPageSetupParameters>(
                new CssPageSetupParameters());
        }

        async void DeleteCss(object commandParameter)
        {
            CSS css = commandParameter as CSS;
            var result = await Services.Popups.TwoOptionPopup
            (
                "Delete Style Sheet?",
                "Permanently delete style sheet?",
                AppResources.AlertOption_Yes,
                AppResources.AlertOption_No
            );

            if (result == TwoOptionPopupResult.LeftButton)
            {
                await Services.NoteDatabase.DeleteAsync(css);
                //UpdateListView();
                ListViewItems.Remove(css);
                RaisePropertyChanged(nameof(ListViewItems));
            }
        }

        async void RenameCss(object commandParameter)
        {
            CSS css = commandParameter as CSS;

            var result = await Services.Popups.PromptPopup
            (
                title: "Rename Style Sheet",
                message: "Input new name for style sheet",
                leftButtonText: "OK",
                rightButtonText: "Cancel",
                initialEntryText: css.Name,
                placeholderText: "Input name..."
            );

            if (result.Selected == TwoOptionPopupResult.LeftButton)
            {
                css.Name = result.Text;
                await Services.NoteDatabase.SaveAsync(css);
                UpdateListView();
            }
        }


    }
}
