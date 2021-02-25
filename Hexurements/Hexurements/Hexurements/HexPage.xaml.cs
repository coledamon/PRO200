using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using Hexurements.Models;
using Android.Graphics;
using Plugin.Media;
using Color = Android.Graphics.Color;

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
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium }) ;
            
            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() =>
                {
                    return photo.GetStream();
                });
                Color color = GetCenterPixel(photo);
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
            
            PhotoImage.Source = ImageSource.FromFile(file.Path);

            Color color = GetCenterPixel(file);
            UpdateHexText(color);
        }

        private void UpdateHexText(Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#")
                .Append(color.R.ToString("X2"))
                .Append(color.G.ToString("X2"))
                .Append(color.B.ToString("X2"));

            Xamarin.Forms.Color colorFromHex = Xamarin.Forms.Color.FromHex(sb.ToString());

            HexText.Text = colorFromHex.ToHex();
            HexText.TextColor = colorFromHex.Luminosity >= .5 ? colorFromHex.WithLuminosity(0) : colorFromHex.WithLuminosity(1);
            HexText.BackgroundColor = colorFromHex;
        }

        public Color GetCenterPixel(Plugin.Media.Abstractions.MediaFile photo)
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(photo.Path);

            int centerX = bitmap.Width / 2;
            int centerY = bitmap.Height / 2;

            int pixel = bitmap.GetPixel(centerX, centerY);

            return new Color(pixel);
        }
    }
}