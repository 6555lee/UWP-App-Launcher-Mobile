using appLauncher.Helpers;
using appLauncher.Model;

using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using Swordfish.NET.Collections;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;
using Windows.UI;

namespace appLauncher.Helpers
{
    public static class GlobalVariables
    {
        /// <summary>
        /// App Tile Settings
        /// </summary>
        public static Color AppForeground { get; set; } = Colors.Black;
        public static Color AppBackground { get; set; } = Colors.LightBlue;
        public static double AppForeGroundOpacity { get; set; } = .25;
        public static double AppBackgroundOpacity { get; set; } = .67;

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
        }
       
        public static async Task LoadCollectionAsync()
        {
           
            //List<finalAppItem> oc1 = AllApps.listOfApps.ToList();
            //ObservableCollection<finalAppItem> oc = new ObservableCollection<finalAppItem>();
            if (await IsFilePresent("collection.txt"))
            {

                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("collection.txt");
                var apps = await Windows.Storage.FileIO.ReadTextAsync(item);
                var allapps = JsonConvert.DeserializeObject<List<finalAppItem>>(apps);
                ConcurrentObservableCollection<finalAppItem> coc = new ConcurrentObservableCollection<finalAppItem>();
                coc.AddRange(allapps);
                AllApps.listOfApps = coc;
            }
            else
            {
                await PackageHelper.GetAllAppsAsync();
            }
          
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
                      
                    }
                    catch (Exception e)
                    {
                      await Logging(e.ToString());
                      Crashes.TrackError(e);
                    }
                if ( await IsFilePresent("BackImageSettings.json"))
                {
                    StorageFile imfile = (StorageFile)await ApplicationData.Current.LocalFolder.TryGetItemAsync("BackImageSettings.txt")
                }
                }
                     

        }
        public static async Task SaveImageOrder()
        {
            if (backgroundImage.Count > 0)
            {
                var imageorder = JsonConvert.SerializeObject(backgroundImage,Formatting.Indented);
                StorageFile item = (StorageFile)await ApplicationData.Current.LocalFolder.CreateFileAsync("images.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(item, imageorder);
            }
           

        }
        
    }
    
    }

