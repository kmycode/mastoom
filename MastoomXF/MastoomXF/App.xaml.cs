using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Mastoom
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

			MainPage = new NavigationPage(new Views.MainPage());
        }

        protected override void OnStart()
		{
			// TODO ID をさらけ出していくスタイル、あとで隠す
			MobileCenter.Start("android=c8a46b87-fa68-4e8a-944b-9a379635e9cd;" + 
			                   "ios=ad44cf63-08b0-481c-9b69-9c468dafbaac",
			                   typeof(Analytics), typeof(Crashes));
			
		}

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
