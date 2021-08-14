using Notes.Services;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Notes.RouteUtil;

namespace Notes.ViewModels
{
    public class RenderPageViewModel : RenderPageViewModelBase, IQueryableViewModel<RenderPageSetupParameters>
    {
        public RenderPageViewModel(IAppServiceProvider services) : base(services)
        {
            SelectCssCommand = new Command(SelectCss);
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var p = new RenderPageSetupParameters();
            p.FromQuery(query);
            Setup(p);
        }

        public async void Setup(RenderPageSetupParameters setupParameters)
        {
            string text = (await Services.NoteDatabase.GetCachedNote()).Text;
            InitialiseHtml(text, setupParameters.FolderId);
        }

        public ICommand SelectCssCommand { get; set; }

        async void SelectCss()
        {
            await Services.Navigation.GoToAsync<CssSelectionPageViewModel>();
        }

        
    }
}
