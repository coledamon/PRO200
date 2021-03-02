using System;
using System.Text;
using Xamarin.Forms;
using Android.Graphics;
using Plugin.Media;
using Color = Android.Graphics.Color;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using PCLStorage;

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
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium }) ;
            
            if (photo != null)
            {
                PhotoImage.Source = ImageSource.FromStream(() =>
                {
                    return photo.GetStream();
                });
                Color color = GetCenterPixel(photo);
                UpdateHexText(color);
                await SaveColorToFile(color);
                await ReadFileExample();
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
            await SaveColorToFile(color);
        }

        private void UpdateHexText(Color color)
        {
            Xamarin.Forms.Color colorFromHex = Xamarin.Forms.Color.FromHex(ColorToHex(color));

            HexText.Text = colorFromHex.ToHex();
            HexText.TextColor = colorFromHex.Luminosity >= .5 ? colorFromHex.WithLuminosity(0) : colorFromHex.WithLuminosity(1);
            HexText.BackgroundColor = colorFromHex;
        }

        private Color GetCenterPixel(MediaFile photo)
        {
            Bitmap bitmap = BitmapFactory.DecodeFile(photo.Path);

            int centerX = bitmap.Width / 2;
            int centerY = bitmap.Height / 2;

            int pixel = bitmap.GetPixel(centerX, centerY);

            return new Color(pixel);
        }

        private async Task SaveColorToFile(Color color)
        {
            var app = App.Current as App;

            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // If folder does exist, open it instead.
            IFolder colorsFolder = await rootFolder.CreateFolderAsync(app.ColorsFolderName,
                CreationCollisionOption.OpenIfExists);

            IFile file = await colorsFolder.CreateFileAsync(app.ColorsFileName, 
                CreationCollisionOption.OpenIfExists);

            string fileContent = await file.ReadAllTextAsync();

            await file.WriteAllTextAsync(fileContent + 
                Environment.NewLine + 
                ColorToHex(color));
        }

        private string ColorToHex(Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#")
                .Append(color.R.ToString("X2"))
                .Append(color.G.ToString("X2"))
                .Append(color.B.ToString("X2"));

            return sb.ToString();
        }

        // Example code to read back the hex color.
        private async Task ReadFileExample()
        {
            // Access application properties for folder/file names.
            var app = App.Current as App;

            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // If folder does exist, open it instead.
            IFolder colorsFolder = await rootFolder.CreateFolderAsync(app.ColorsFolderName,
                CreationCollisionOption.OpenIfExists);

            IFile file = await colorsFolder.CreateFileAsync(app.ColorsFileName,
                CreationCollisionOption.OpenIfExists);

            // You now have file's content
            string content = await file.ReadAllTextAsync();

        }
    }
}