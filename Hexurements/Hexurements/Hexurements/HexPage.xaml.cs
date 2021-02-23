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
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());
            
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

            HexText.Text = sb.ToString();
            HexText.BackgroundColor = Xamarin.Forms.Color.FromHex(sb.ToString());
        }

        public Color GetCenterPixel(Plugin.Media.Abstractions.MediaFile photo)
        {
            var memoryStream = new MemoryStream();
            photo.GetStream().CopyTo(memoryStream);

            byte[] imageBytes = memoryStream.ToArray();

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

            int pixel = bitmap.GetPixel(
                (int)PhotoImage.Width / 2,
                (int)PhotoImage.Height / 2);

            return new Color(pixel);
        }
    }
}