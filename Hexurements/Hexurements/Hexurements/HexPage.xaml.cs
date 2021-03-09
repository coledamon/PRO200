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
using SkiaSharp.Views.Forms;

namespace Hexurements
{
    public partial class HexPage : ContentPage
    {

        ObservableCollection<Hex> hexes = new ObservableCollection<Hex>();
        SKBitmap sKBitmap;
        int crossSize = 50;

        public HexPage()
        {
            
            InitializeComponent();
            ColorList.ItemsSource = hexes;
            LoadHexes();
            skPhotoImage.InvalidateSurface();
            ColorList.ItemSelected += async (s, e) =>
            {
                Hex selectedHex = (Hex)ColorList.SelectedItem;
                var action = await DisplayActionSheet("", "Cancel", null, "Delete Hex", "Clear List");
                switch (action)
                {
                    case "Delete Hex":
                        DeleteHex(selectedHex);
                        break;
                    case "Clear List":
                        ClearList();
                        break;
                }
            };
        }

        private async void DeleteHex(Hex hex)
        {
            var action = await DisplayActionSheet("Are you sure?", "Cancel", null, "Delete Hex");
            switch (action)
            {
                case "Delete Hex":
                    hexes.Remove(hex);
                    RemoveColorFromFile(hex);
                    break;
            }
        }
        private async void ClearList()
        {
            var action = await DisplayActionSheet("Are you sure?", "Cancel", null, "Clear List");
            switch (action)
            {
                case "Clear List":
                    hexes.Clear();
                    var app = App.Current as App;

                    IFolder rootFolder = FileSystem.Current.LocalStorage;

                    IFolder colorsFolder = await rootFolder.CreateFolderAsync(app.ColorsFolderName,
                        CreationCollisionOption.OpenIfExists);

                    IFile file = await colorsFolder.CreateFileAsync(app.ColorsFileName,
                        CreationCollisionOption.OpenIfExists);

                    await file.WriteAllTextAsync("");

                    break;

            }
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            // Documentation for this: https://www.xamarinhelp.com/use-camera-take-photo-xamarin-forms/
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium });

            if (photo != null)
            {
                sKBitmap = SKBitmap.Decode(photo.GetStream());

                Color color = GetCenterPixel(photo);
                Hex h = new Hex() { ListedColor = Xamarin.Forms.Color.FromHex(ColorToHex(color)) };
                if(hexes.Count(hex => hex.ListedColor == h.ListedColor) == 0) hexes.Add(h);
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

            sKBitmap = SKBitmap.Decode(file.GetStream());

            Color color = GetCenterPixel(file);
            Hex h = new Hex() { ListedColor = Xamarin.Forms.Color.FromHex(ColorToHex(color)) };
            if(hexes.Count(hex => hex.ListedColor == h.ListedColor) == 0) hexes.Add(h);
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
        private async void RemoveColorFromFile(Hex hex)
        {
            var app = App.Current as App;

            IFolder rootFolder = FileSystem.Current.LocalStorage;

            IFolder colorsFolder = await rootFolder.CreateFolderAsync(app.ColorsFolderName,
                CreationCollisionOption.OpenIfExists);

            IFile file = await colorsFolder.CreateFileAsync(app.ColorsFileName,
                CreationCollisionOption.OpenIfExists);

            string fileContent = await file.ReadAllTextAsync();

            string hexCode = hex.Data;

            string newHexCode = hexCode.Remove(1, 2);
            
            fileContent = fileContent.Replace(newHexCode, "");

            await file.WriteAllTextAsync(fileContent);
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

        private void skPhotoImage_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
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

                SKColor color = sKBitmap.GetPixel(sKBitmap.Width / 2, sKBitmap.Height / 2);

                SKPaint paint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Black, StrokeWidth = 5 };
                SKPath path = new SKPath();
                float centerWidth = (scale * sKBitmap.Width / 2) + left;
                float centerHeight = (scale * sKBitmap.Height / 2) + top;

                path.MoveTo(centerWidth, centerHeight - crossSize);
                path.LineTo(centerWidth, centerHeight + crossSize);

                path.MoveTo(centerWidth - crossSize, centerHeight);
                path.LineTo(centerWidth + crossSize, centerHeight);

                canvas.DrawPath(path, paint);

                canvas.DrawRect(new SKRect(centerWidth - 2, centerHeight - 2, centerWidth + 2, centerHeight + 2), new SKPaint() { Style = SKPaintStyle.Stroke, Color = color, StrokeWidth = 1 });

            }
            else
            {
                SKPaint paint = new SKPaint();
                Xamarin.Forms.Color color = (Xamarin.Forms.Color)Application.Current.Resources["TextColor"];
                paint.Color = color.ToSKColor();
                paint.TextAlign = SKTextAlign.Center;
                paint.TextSize = 48;

                canvas.DrawText("Upload or Take a Photo",
                    e.Info.Width/2, e.Info.Height / 2, paint);
            }
        }
    }
}