using appLauncher.Model;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace appLauncher.Helpers
{
    public static class GlobalVariables
    {
        /// <summary>
        /// App Tile Settings
        /// </summary>


        /// <summary>
        /// Background Image Settings
        /// </summary>
        public static ObservableCollection<BackgroundImages> backgroundImage { get; set; } = new ObservableCollection<BackgroundImages>();
        public static Color ImageColor { get; set; } = Colors.Transparent;
        public static double ImageOpacity { get; set; } = .25;


        /// <summary>
        /// Main Page Settings
        /// </summary>
        public static int appsperscreen { get; set; }
        public static int NumofRoworColumn(int padding, int objectsize, int sizetofit)
        {
            int amount = 0;
            int intsize = objectsize + (padding + padding);
            int size = intsize;
            while (size + intsize < sizetofit)
            {
                amount += 1;
                size += intsize;
            }
            return amount;

        }
        public static int columns { get; set; }



        /// <summary>
        /// AppControl Control Settings
        /// </summary>
        public static int oldindex { get; set; }
        public static int newindex { get; set; }
        public static int pagenum { get; set; }
        public static finalAppItem itemdragged { get; set; }
        public static Point startingpoint { get; set; }
        public static bool isdragging { get; set; }


        /// <summary>
        /// Global Variables & Functions
        /// </summary>
        public static bool bgimagesavailable { get; set; }

        public static async Task Logging(string texttolog)
        {
            StorageFolder stors = ApplicationData.Current.LocalFolder;
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), texttolog);
            await FileIO.AppendTextAsync(await stors.CreateFileAsync("logfile.txt", CreationCollisionOption.OpenIfExists), Environment.NewLine);
            Analytics.TrackEvent(texttolog);
        }

        public static async Task<List<finalAppItem>> LoadCollectionAsync()
        {

            List<finalAppItem> oc1 = new List<finalAppItem>();
            //ObservableCollection<finalAppItem> oc = new ObservableCollection<finalAppItem>();
            try
            {

                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                string apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                oc1 = JsonConvert.DeserializeObject<List<finalAppItem>>(apps);

            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                await Logging(e.Message);
            }
            return oc1;

        }
        public static async Task SaveCollectionAsync()
        {

            if (AllApps.listOfApps.Count > 0)
            {
                var te = JsonConvert.SerializeObject(AllApps.listOfApps, Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("collection.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, te);
            }

        }
        public static async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }
        public static async Task LoadBackgroundImages()
        {
            if (await IsFilePresent("images.txt"))
            {
                try
                {
                    StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("images.txt");
                    var images = await FileIO.ReadTextAsync(item);
                    backgroundImage = JsonConvert.DeserializeObject<ObservableCollection<BackgroundImages>>(images);
                    foreach (var items in backgroundImage)
                    {
                        BitmapImage bmi = await GlobalVariables.ConvertfromByteArraytoBitmapImage(items.ImageData);
                        items.BitmapImageImage = bmi;
                    }


                }
                catch (Exception e)
                {
                    await Logging(e.ToString());
                    Crashes.TrackError(e);
                }

            }


        }
        public static async Task SaveImageOrder()
        {
            if (backgroundImage.Count > 0)
            {
                var imageorder = JsonConvert.SerializeObject(backgroundImage, Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, imageorder);
            }


        }

        public static async Task<BitmapImage> ReturnImage(StorageFile filename)
        {
            var logoStream = RandomAccessStreamReference.CreateFromFile(filename);
            BitmapImage image = new BitmapImage();
            using (IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync())
            {
                await image.SetSourceAsync(whatIWant);
            }
            return image;

        }

        public static async Task<string> ConvertImageFiletoByteArrayAsync(StorageFile filename)
        {
            using (var inputStream = await filename.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                byte[] buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }
        public static async Task<BitmapImage> ConvertfromByteArraytoBitmapImage(string imagestr)
        {
            byte[] bytes = Convert.FromBase64String(imagestr);
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(bytes);
                    await writer.StoreAsync();
                }
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(stream);
                return image;
            }
        }

    }

}

