using Notes.RouteUtil;
using Notes.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notes.Services
{
    public interface INavigationService
    {
        Page CurrentPage { get; }

        Task GoBackAsync();
        Task GoToAsync<TViewModel, TSetupParameters>(TSetupParameters setupParamters)
            where TViewModel : IQueryableViewModel<TSetupParameters>
            where TSetupParameters : IQueryConvertable;
        Task GoToAsync<TViewModel>() where TViewModel : IViewModel;
        Task GoToAsync(ShellNavigationState state, bool animate = true);
    }
}