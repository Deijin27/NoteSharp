using Notes.Models;
using Notes.Services;
using Notes.Resources;

namespace Notes.ViewModels
{
    public class CssRenderPageViewModel : RenderPageViewModelBase
    {
        public CssRenderPageViewModel(IAppServiceProvider services) : base(services)
        {
            var css = Services.NoteDatabase.GetCachedCss().Result;
            ParseMarkdownThenUpdate(AppResources.MarkdownReferenceAndTest, css);
        }
    }
}
