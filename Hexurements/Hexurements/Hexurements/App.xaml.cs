using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hexurements
{
    public partial class App : Application
    {
        private const string colorsFolderName = "Unlocked Colors";
        private const string colorsFileName = "unlockedColors.txt";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public string ColorsFolderName
        {
            get
            {
                return colorsFolderName;
            }
        } 

        public string ColorsFileName
        {
            get
            {
                return colorsFileName;
            }
        }
    }
}
