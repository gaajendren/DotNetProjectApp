using DotNetProjectApp.Views;

namespace DotNetProjectApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
        }

      
    }
}
