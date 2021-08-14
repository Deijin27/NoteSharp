using Notes.Models;
using Notes.RouteUtil;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels.Base
{
    public abstract class EntryViewModelBase<TSetupParameters, TEntry> : QueryableViewModelBase<TSetupParameters> 
        where TSetupParameters : IQueryConvertable, new()
        where TEntry : IEntry
    {
        protected IAppServiceProvider Services;
        public EntryViewModelBase(IAppServiceProvider services)
        {
            Services = services;

            SaveCommand = new Command(async () => await Save());
            CloseCommand = new Command(async () => await Close());
            RenderCommand = new Command(Render);
        }

        protected bool UnsavedChangesExist = false;
        protected bool IsNew = false;

        protected TEntry Current { get; set; }

        public string CurrentName
        {
            get => Current.Name;
            set
            {
                if (value != Current.Name)
                {
                    Current.Name = value;
                    UnsavedChangesExist = true;
                }
            }
        }
        public string CurrentText
        {
            get => Current.Text;
            set
            {
                if (value != Current.Text)
                {
                    Current.Text = value;
                    UnsavedChangesExist = true;
                }
            }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand RenderCommand { get; set; }

        async Task Close()
        {
            if (UnsavedChangesExist)
            {
                var result = await Services.Popups.TwoOptionPopup
                (
                    "Save Changes?",
                    "Unsaved changes exist, would you like to save them?",
                    "Yes",
                    "No"
                );

                if (result == TwoOptionPopupResult.LeftButton)
                {
                    await Save();
                }
            }
            await Services.Navigation.GoBackAsync();
        }
        protected abstract Task Save();
        protected abstract void Render();

        public abstract Task Cache();

        protected abstract Task RestoreCacheIfExists();

        /// <summary>
        /// Called on hardware back
        /// </summary>
        /// <returns>Whether back navigation was handled by this</returns>
        public async void OnHardwareBackPressed() // maybe only necessary in modal pages
        {
            await Close();
        }
    }
}
