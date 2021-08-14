using System;
using Xamarin.Forms;
using Notes.Pages;
using Notes.Services;
using Notes.ViewModels;
using Notes.RouteUtil;
using Notes.ViewModels.Base;

namespace Notes
{

    public partial class App : Application
    {
        readonly IAppServiceProvider Services;
        public App()
        {
            InitializeComponent();

            // create service provider
            Services = AppServiceProvider.Instance;
            Services.Theme.UpdateMergedDictionaries(); // why did I do this? Does it load the theme?

            // Create main page
            var mainPage = new MainShell();
            MainPage = mainPage;

            // Give reference of shell to navigationservice
            var nav = AppServiceProvider.Instance.NavigationInstance;
            nav.ShellInstance = mainPage;

            // register routes for all pages that will be navigated to not via flyout
            nav.RegisterRoute<CssPage, CssPageViewModel>(Routes.Css);
            nav.RegisterRoute<CssRenderPage, CssRenderPageViewModel>(Routes.CssRender);
            nav.RegisterRoute<CssSelectionPage, CssSelectionPageViewModel>(Routes.CssSelection);
            nav.RegisterRoute<FolderPage, FolderPageViewModel>(Routes.Folder);
            nav.RegisterRoute<MovePage, MovePageViewModel>(Routes.Move);
            nav.RegisterRoute<NotePage, NotePageViewModel>(Routes.Note);
            nav.RegisterRoute<PreviewPage, PreviewPageViewModel>(Routes.Preview);
            nav.RegisterRoute<RenderPage, RenderPageViewModel>(Routes.Render);

            //var tatt = Services.Preferences.Uri;
            //Services.Preferences.Uri = "folder?folderid=034509d9-0ed6-42a6-b713-2d78457139e4/note?noteid=8eaafcc4-c7a0-4dc5-95b6-89b2c011b5b6";
            //Services.Preferences.Uri = "";
            //foreach (string uriPart in Services.Preferences.Uri.Split('/'))
            //{
            //    Services.Navigation.GoToAsync(uriPart);
            //}
            
        }

        // WHY NO DO WHAT I WANT AAAAAAAAAAAA???????????
        // WHY NO DO WHAT I WANT AAAAAAAAAAAA???????????
        // WHY NO DO WHAT I WANT AAAAAAAAAAAA???????????

        protected override void OnStart()
        {
            // Handle when your app starts 
            base.OnStart();
            // navigate to cached uri
            //var path = Services.Preferences.Uri;
            //if (path != null)
            //{
            //    Services.Navigation.GoToAsync(path);
            //}
            // maybe do this as a try catch in case something goes wrong and they get stuck in a loop
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            base.OnSleep();
            // cache current uri and call cachetext for noteviewmodels
            //Services.Preferences.Uri = Services.Navigation.CurrentUri;
            //if (Services.Navigation.CurrentPage.BindingContext is ICacheable cacheable)
            //{
            //    //await cacheable.Cache();
            //}
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            base.OnResume();
            // navigate to cached uri (if necessary?)
            //var path = Services.Preferences.Uri;
            //if (path != null)
            //{
            //    Services.Navigation.GoToAsync(path);
            //}
        }
    }
}
