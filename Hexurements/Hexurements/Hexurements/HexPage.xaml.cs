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
using Plugin.Media;

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

        private void UploadButton_Clicked(object sender, EventArgs e)
        {
            UploadPhoto();
        }

        private async void UploadPhoto()
        {
            await CrossMedia.Current.Initialize();

            // Uploading photos is not supported
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                //Toast.MakeText(this, "Upload not supported on this device");
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40
            }) ;

            //byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            
            PhotoImage.Source = ImageSource.FromFile(file.Path);
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