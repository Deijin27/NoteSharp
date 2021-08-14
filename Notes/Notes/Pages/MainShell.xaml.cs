using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainShell : Shell
    {
        public MainShell()
        {
            InitializeComponent();
        }

        public void RegisterRoute<TPage>(string route) where TPage : Page
        {
            Routing.RegisterRoute(route, typeof(TPage));
        }
    }
}