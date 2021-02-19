using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hexurements
{
    public partial class MainPage : ContentPage
    {
        private bool ogColor = true;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Style_Clicked(object sender, EventArgs e)
        {
            App.Current.Resources["TextColor"] = ogColor ? App.Current.Resources["AltTextColor"] : App.Current.Resources["OGTextColor"];
            App.Current.Resources["BgColor"] = ogColor ? App.Current.Resources["AltBgColor"] : App.Current.Resources["OGBgColor"];
            App.Current.Resources["BtnColor"] = ogColor ? App.Current.Resources["AltBtnColor"] : App.Current.Resources["OGBtnColor"];
            StyleBtn.ImageSource = ogColor ? "sun.png" : "moon.png";
            ogColor = !ogColor;
        }

        private async void HexPicker_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HexPage());
        }

        private async void MeasuringTool_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MeasurePage());
        }
    }
}
