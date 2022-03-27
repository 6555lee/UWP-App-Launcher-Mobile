using System;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using appLauncher.Core.Brushes;

namespace appLauncher.Model
{
    public class BackgroundImages
    {
       public string filename { get; set; }="test.png";
       public Byte[] bitmapImage { get; set; } = new byte[1];
    public MaskedBrush BackImageAsync()
        {
           // CompositionColorBrush comp = Window.Current.Compositor.CreateColorBrush(ColorHelper.FromArgb(this.ImageColor.A, this.ImageColor.R, this.ImageColor.G, this.ImageColor.B));
           // var image = new BitmapImage();
           // using (var stream = new InMemoryRandomAccessStream())
           // {
           //     stream.WriteAsync(bitmapImage.AsBuffer()).GetResults();
           //     // I made this one synchronous on the UI thread;
           //     // this is not a best practice.
               
           //     stream.Seek(0);
           //     image.SetSource(stream);
           //}
           // ImageBrush ib = new ImageBrush();
           // ib.ImageSource = image;
            MaskedBrush mb = new MaskedBrush(bitmapImage,ImageColor);
            return mb;
        }
       
    }
    public class BackgroundImageSettings
    {
        public Color imageColor { get; set; } = Colors.Blue;
        public double imageOpacity { get; set; } = .35;
    }
}
