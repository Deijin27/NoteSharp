using Notes.RouteUtil;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Notes.ViewModels
{
    public class FolderPageViewModel : FolderViewModelBase, IQueryableViewModel<FolderPageSetupParameters>
    {
        public FolderPageViewModel(IAppServiceProvider services) : base(services) 
        {
            
        }

        public override void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var p = new FolderPageSetupParameters();
            p.FromQuery(query);
            Setup(p);
        }

        public async void Setup(FolderPageSetupParameters setupParameters)
        {
            if (setupParameters.FolderId == Guid.Empty)
            {
                PageTitle = "Root";
            }
            else
            {
                PageTitle = (await Services.NoteDatabase.GetFolderAsync(setupParameters.FolderId)).Name;
            }
            CurrentFolderId = setupParameters.FolderId;
            UpdateListView();
        }
    }
}
