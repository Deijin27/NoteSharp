using Notes.RouteUtil;
using System.Collections.Generic;

namespace Notes.ViewModels
{
    public abstract class QueryableViewModelBase<TSetupParameters> : ViewModelBase, IQueryableViewModel<TSetupParameters> 
        where TSetupParameters : IQueryConvertable, new()
    {
        public abstract void Setup(TSetupParameters setupParameters);

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var p = new TSetupParameters();
            p.FromQuery(query);
            Setup(p);
        }
    }
}
