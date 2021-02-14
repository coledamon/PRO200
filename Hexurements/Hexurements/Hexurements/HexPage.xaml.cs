using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hexurements
{
    public partial class HexPage : ContentPage
    {
        public HexPage()
        {
            InitializeComponent();
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            // Documentation for this: https://www.xamarinhelp.com/use-camera-take-photo-xamarin-forms/
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() =>
                {
                    return photo.GetStream();
                });
                CameraButton.Text = "Take New Picture";
                HexText.Text = "#000000";
            }
        }
    }
}