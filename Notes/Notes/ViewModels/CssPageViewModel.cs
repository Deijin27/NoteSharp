using Notes.Models;
using Notes.RouteUtil;
using Notes.Services;
using Notes.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public class CssPageViewModel : EntryViewModelBase<CssPageSetupParameters, CSS>, ICacheable
    {
        public CssPageViewModel(IAppServiceProvider services) : base(services)
        {
            ColorPickerCommand = new Command(ColorPicker);
            CopyCommand = new Command(Copy);
        }

        public async override void Setup(CssPageSetupParameters setupParameters)
        {
            var css = await Services.StyleSheets.GetStyleSheetAsync(setupParameters.CssId);
            if (css == null) // null return is default, i.e. note doesn't exist
            {
                Current = new CSS
                {
                    ID = setupParameters.CssId // important, ensures note is saved as this id, so the page will reload correctly on restart
                };
                IsNew = true;
            }
            else
            {
                Current = css;
            }
            IsReadOnly = Current.IsReadOnly; // do before restoring cache
            await RestoreCacheIfExists();
        }

        protected async override Task Save()
        {
            if (UnsavedChangesExist || IsNew)
            {
                await Services.NoteDatabase.SaveAsync(Current);

                UnsavedChangesExist = false;
                IsNew = false;
            }
        }

        protected async override void Render()
        {
            await Cache();
            await Services.Navigation.GoToAsync<CssRenderPageViewModel>();
        }

        private bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => RaiseAndSetIfChanged(ref _isReadOnly, value);
        }

        public ICommand ColorPickerCommand { get; set; }

        public async void ColorPicker()
        {
            await Services.Popups.ColorPickerPopup();
        }

        public ICommand CopyCommand { get; set; }

        public async void Copy()
        {
            await Services.Clipboard.SetTextAsync(CurrentText);
        }

        public override async Task Cache()
        {
            await Services.NoteDatabase.SetCachedCss(new CSS() { Name = Current.Name, Text = CurrentText, IsReadOnly = IsReadOnly });
        }

        protected override async Task RestoreCacheIfExists()
        {
            CSS cache = await Services.NoteDatabase.GetCachedCss();
            if (cache != null)
            {
                CurrentText = cache.Text;
                CurrentName = cache.Name;
                IsReadOnly = cache.IsReadOnly;
                await Services.NoteDatabase.ClearCachedNote();
            }
            RaisePropertyChanged(nameof(CurrentName));
            RaisePropertyChanged(nameof(CurrentText));
        }
    }
}
