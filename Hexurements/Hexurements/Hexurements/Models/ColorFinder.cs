using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using Plugin.Media.Abstractions;
using System.IO;
using System.Drawing;
using Android.Graphics;
using Color = Android.Graphics.Color;

namespace Hexurements.Models
{
    //public class ColorFinder
    //{
    //    private readonly MemoryStream photoStream;

    //    public ColorFinder(MemoryStream stream)
    //    {
    //        this.photoStream = stream;
    //    }

    //    public Color GetCenterPixel(MediaFile photo)
    //    {
    //        if (photoStream == null)
    //            throw new ArgumentNullException("You must set this object with a memory stream.");

    //        byte[] imageBytes = this.photoStream.ToArray();
            
    //        Image image = new Image()
    //        {
    //            Source = ImageSource.FromStream(() => { return photo.GetStream(); })
    //        };
            
    //        Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

    //        int width = (int) image.Width;
    //        int height = (int)image.Height;

    //        double widthD = image.Width;
    //        double heightD = image.Height;

    //        width /= 2;
    //        height /= 2;

    //        int pixel = bitmap.GetPixel(
    //            (int)image.Width / 2,
    //            (int)image.Height / 2);

    //        return new Color(pixel);
    //    }
    //}
}
