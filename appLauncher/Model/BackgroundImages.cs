using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using appLauncher.Core.Brushes;
using Windows.UI.Xaml.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Streams;

namespace appLauncher.Model
{
   public class BackgroundImages
    {
       public string filename { get; set; }="Default File Name";
       public Byte[] bitmapImage { get; set; } = new byte[1];
       public Color ImageColor { get; set; } = Colors.Blue;
       public double ImageOpacity { get;set; } = .25;
       public ImageBrush BackImageAsync()
        {
            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                stream.WriteAsync(bitmapImage.AsBuffer()).GetResults();
                // I made this one synchronous on the UI thread;
                // this is not a best practice.
               
                stream.Seek(0);
                image.SetSource(stream);
           }
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = image;
            return ib ;
        }
       
    }
}
