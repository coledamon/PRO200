using System;
using System.Text;
using Xamarin.Forms;
using Android.Graphics;
using Plugin.Media;
using Color = Android.Graphics.Color;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using PCLStorage;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SkiaSharp;

namespace Hexurements
{
    public partial class HexPage : ContentPage
    {

        ObservableCollection<Hex> hexes = new ObservableCollection<Hex>();
        SKBitmap sKBitmap;

        public HexPage()
        {
            
            InitializeComponent();
            ColorList.ItemsSource = hexes;
            LoadHexes();
            skPhotoImage.InvalidateSurface();
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            // Documentation for this: https://www.xamarinhelp.com/use-camera-take-photo-xamarin-forms/
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium });

            if (photo != null)
            {
                sKBitmap = SKBitmap.Decode(photo.GetStream());

                //PhotoImage.Source = ImageSource.FromStream(() =>
                //{
                //    return photo.GetStream();
                //});
                Color color = GetCenterPixel(photo);
                Hex h = new Hex() { ListedColor = Xamarin.Forms.Color.FromHex(ColorToHex(color)) };
                hexes.Add(h);
                UpdateHexText(color);
                await SaveColorToFile(color);
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
            });

            //PhotoImage.Source = ImageSource.FromFile(file.Path);
            sKBitmap = SKBitmap.Decode(file.GetStream());

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

            string colorHex = ColorToHex(color);

            if (!fileContent.Contains(colorHex))
            {
                await file.WriteAllTextAsync(fileContent +
                    Environment.NewLine +
                    colorHex);
            }

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
        private async Task LoadHexes()
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
            string[] readHexes = content.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries
                );
            hexes.Clear();
            foreach (string hex in readHexes)
            {
                Hex h = new Hex() { ListedColor = Xamarin.Forms.Color.FromHex(hex) };
                hexes.Add(h);
            }
        }


        public class Hex : INotifyPropertyChanged
        {
            private Xamarin.Forms.Color listedColor;
            private Xamarin.Forms.Color listedColorAlt;

            public Xamarin.Forms.Color ListedColor
            {
                get { return listedColor; }
                set
                {
                    if (value != listedColor)
                    {
                        listedColor = value;
                        ListedColorAlt = listedColor.Luminosity >= .5 ? listedColor.WithLuminosity(0) : listedColor.WithLuminosity(1);
                        onPropertyChanged("ListedColor");
                    }
                }
            }
            public Xamarin.Forms.Color ListedColorAlt
            {
                get { return listedColorAlt; }
                set
                {
                    if (value != listedColorAlt)
                    {
                        listedColorAlt = value;
                        onPropertyChanged("ListedColorAlt");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void onPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public string Data
            {
                get
                {
                    return ListedColor.ToHex();
                }
            }
        }

        private void skPhotoImage_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            SKImageInfo info = e.Info;
            skPhotoImage.InvalidateSurface();

            if (sKBitmap != null)
            {
                float scale = (float)info.Width*2 / sKBitmap.Width/3;

                float left = (info.Width - scale * sKBitmap.Width) / 2;
                float top = (info.Height - scale * sKBitmap.Height) / 2;
                float right = left + scale * sKBitmap.Width;
                float bottom = top + scale * sKBitmap.Height;
                SKRect rect = new SKRect(left, top, right, bottom);

                canvas.DrawBitmap(sKBitmap, rect);
                //canvas.DrawBitmap(sKBitmap, e.Info.Width/4, e.Info.Height/4);
            }
            else
            {
                SKPaint paint = new SKPaint();                
                paint.Color = SKColors.Blue;
                paint.TextAlign = SKTextAlign.Center;
                paint.TextSize = 48;

                canvas.DrawText("Upload or Take a Photo",
                    e.Info.Width/2, e.Info.Height / 2, paint);
                
            }
        }
    }
}