using Notes.Pages;
using Notes.RouteUtil;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notes.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IAppServiceProvider Services;
        public NavigationService(IAppServiceProvider services)
        {
            Services = services;
        }
        public MainShell ShellInstance;

        private readonly Dictionary<Type, string> VmToRouteLookup = new Dictionary<Type, string>();

        public void RegisterRoute<TPage, TViewModel>(string route) where TViewModel : IViewModel where TPage : Page
        {
            VmToRouteLookup[typeof(TViewModel)] = route;
            ShellInstance.RegisterRoute<TPage>(route);
        }

        public async Task GoToAsync(ShellNavigationState state, bool animate = true)
        {
            await ShellInstance.GoToAsync(state, animate).ConfigureAwait(false);
        }

        public async Task GoToAsync<TViewModel, TSetupParameters>(TSetupParameters setupParamters) 
            where TSetupParameters : IQueryConvertable
            where TViewModel : IQueryableViewModel<TSetupParameters>
        {
            string path = VmToRouteLookup[typeof(TViewModel)] + setupParamters.ToQuery();
            await ShellInstance.GoToAsync(path, true).ConfigureAwait(false);
            Services.Preferences.Uri += "/" + path;
        }

        public async Task GoToAsync<TViewModel>()
            where TViewModel : IViewModel
        {
            string path = VmToRouteLookup[typeof(TViewModel)];
            await ShellInstance.GoToAsync(path, true).ConfigureAwait(false);
            Services.Preferences.Uri += "/" + path;
        }

        public async Task GoBackAsync()
        {
            await ShellInstance.GoToAsync("..", true).ConfigureAwait(false);
            var uri = Services.Preferences.Uri;
            if (uri.Contains('/'))
            {
                Services.Preferences.Uri = string.Join('/', uri.Split('/')[..^1]);
            }
            else
            {
                uri = "";
            }
        }

        public Page CurrentPage => ShellInstance.CurrentPage;
    }
}
