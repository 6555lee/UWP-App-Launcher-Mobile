using System;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using appLauncher.Core.Brushes;
using appLauncher.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace appLauncher.Model
{
    public class BackgroundImages
    {
       public string Filename { get; set; }="test.png";
        [JsonIgnore]
       public BitmapImage BitmapImageImage { get; set; } = new BitmapImage();
        public string ImageData { get; set; }
    }
   
}
 