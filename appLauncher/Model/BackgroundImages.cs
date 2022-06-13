using Newtonsoft.Json;

using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Model
{
    public class BackgroundImages
    {
        public string Filename { get; set; } = "test.png";
        [JsonIgnore]
        public BitmapImage BitmapImageImage { get; set; } = new BitmapImage();
        public string ImageData { get; set; }
    }

}
