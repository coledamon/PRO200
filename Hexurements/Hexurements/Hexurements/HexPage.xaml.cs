using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using Hexurements.Models;
using Android.Graphics;
using Color = Android.Graphics.Color;


namespace Hexurements
{
    public partial class HexPage : ContentPage
    {
        private byte[] arr = new byte[5];
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
                Color color;
                ColorFinder finder = null;

                PhotoImage.Source = ImageSource.FromStream(() =>
                {
                    var memoryStream = new MemoryStream();
                    photo.GetStream().CopyTo(memoryStream);

                    finder = new ColorFinder(memoryStream);

                    return photo.GetStream();
                });

                color = finder.GetCenterPixel();
                UpdateHexText(color);
            }
        }

        private void UpdateHexText(Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#")
                .Append(color.R.ToString("X2"))
                .Append(color.G.ToString("X2"))
                .Append(color.B.ToString("X2"));

            HexText.Text = sb.ToString();
            HexText.BackgroundColor = Xamarin.Forms.Color.FromHex(sb.ToString());
        }
    }
}