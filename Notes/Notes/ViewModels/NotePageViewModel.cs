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
    public class NotePageViewModel : EntryViewModelBase<NotePageSetupParameters, Note>, ICacheable
    {
        public NotePageViewModel(IAppServiceProvider services) : base(services)
        {
            HtmlPreviewCommand = new Command(HtmlPreview);
            MarkdownPreviewCommand = new Command(MarkdownPreview);

            IsSpellCheckEnabled = Services.Preferences.IsSpellCheckEnabled;
        }

        public bool IsSpellCheckEnabled { get; private set; }

        public async override void Setup(NotePageSetupParameters parameters)
        {
            var note = await Services.NoteDatabase.GetNoteAsync(parameters.NoteId);
            if (note == null) // null return is default, i.e. note doesn't exist
            {
                Current = Note.New(parameters.FolderId);
                Current.ID = parameters.NoteId; // important, ensures note is saved as this id, so the page will reload correctly on restart
                IsNew = true;
            }
            else
            {
                Current = note;
            }
            await RestoreCacheIfExists();
        }

        protected async override Task Save()
        {
            if (UnsavedChangesExist || IsNew)
            {
                Current.DateModified = DateTime.UtcNow;
                if (Current.DateCreated == default)
                {
                    Current.DateCreated = Current.DateModified;
                }
                await Services.NoteDatabase.SaveAsync(Current);

                UnsavedChangesExist = false;
                IsNew = false;
            }
        }

        public ICommand HtmlPreviewCommand { get; set; }
        public ICommand MarkdownPreviewCommand { get; set; }

        async void HtmlPreview()
        {
            await Cache();
            await Services.Navigation.GoToAsync<PreviewPageViewModel, PreviewPageSetupParameters>(
                new PreviewPageSetupParameters(Current.FolderID, PreviewPageMode.Html));
        }

        async void MarkdownPreview()
        {
            await Cache();
            await Services.Navigation.GoToAsync<PreviewPageViewModel, PreviewPageSetupParameters>(
                new PreviewPageSetupParameters(Current.FolderID, PreviewPageMode.Markdown));
        }

        protected override async void Render()
        {
            await Cache();
            await Services.Navigation.GoToAsync<RenderPageViewModel, RenderPageSetupParameters>(
                new RenderPageSetupParameters(Current.FolderID));
        }


        public override async Task Cache()
        {
            await Services.NoteDatabase.SetCachedNote(new Note() { Name = Current.Name, Text = CurrentText });
        }

        protected override async Task RestoreCacheIfExists()
        {
            Note cache = await Services.NoteDatabase.GetCachedNote();
            if (cache != null)
            {
                CurrentText = cache.Text;
                CurrentName = cache.Name;
                await Services.NoteDatabase.ClearCachedNote();
            }
            RaisePropertyChanged(nameof(CurrentName));
            RaisePropertyChanged(nameof(CurrentText));
        }
    }
}
