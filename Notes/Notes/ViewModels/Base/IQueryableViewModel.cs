using Xamarin.Forms;
using Notes.RouteUtil;

namespace Notes.ViewModels
{
    public interface IQueryableViewModel<TSetupParameters> : IViewModel, IQueryAttributable where TSetupParameters : IQueryConvertable
    {
        public void Setup(TSetupParameters setupParameters);
    }
    
}
