// Methods for getting installed apps/games from the device are here. Note: Package = App/Game
using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.StartScreen;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;

namespace appLauncher.Helpers
{
   public static class PackageHelper
   {
		
		
		
       public static PackageManager pkgManager = new PackageManager();
       public static event EventHandler AppsRetreived;
        /// <summary>
        /// Gets app package and image and returns them as a new "finalAppItem" asynchronously, which will then be used for the app control template.
        /// <para> Of the two getAllApps() methods, this is the preferred version because it doesn't block the stop the rest of the app from running when 
        /// this is being run.</para>
        /// </summary>
        /// <returns></returns>

        public static async Task GetAllAppsAsync()
        {
            if (await GlobalVariables.IsFilePresent("collection.txt"))
            {
                await GlobalVariables.LoadCollectionAsync();
                GlobalVariables.AppForeground = AllApps.listOfApps[0].ForegroundColor;
                GlobalVariables.AppForeGroundOpacity = AllApps.listOfApps[0].ForegroundOpacity;
                GlobalVariables.AppBackground = AllApps.listOfApps[0].BackgroundColor;
                GlobalVariables.AppBackgroundOpacity = AllApps.listOfApps[0].BackgroundOpacity;
                AppsRetreived?.Invoke(true, EventArgs.Empty);
            }
            else { 
                IEnumerable<Package> listOfInstalledPackages = pkgManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);
                List<Package> allPackages = listOfInstalledPackages.ToList();
                foreach (Package item in allPackages)
                {
                    try
                    {
                        IReadOnlyList<AppListEntry> readonlylist = await item.GetAppListEntriesAsync();
                        List<AppListEntry> applistentries = readonlylist.ToList();
                        if (applistentries.Count > 0)
                        {
                            Debug.WriteLine("YES!");
                        }
                        else
                        {
                            continue;
                        }
                        AppListEntry applist = applistentries[0];

                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                        {
                            //UI code here
                            try
                            {
                                RandomAccessStreamReference logoStream;
                                byte[] temp;
                                try
                                {
                                    logoStream = applist.DisplayInfo.GetLogo(new Size(50, 50));

                                    IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
                                    temp = new byte[whatIWant.Size];
                                    using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
                                    {
                                        await read.LoadAsync((uint)whatIWant.Size);
                                        read.ReadBytes(temp);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.Write(e.ToString());
                                    Crashes.TrackError(e);
                                    temp = new byte[1];
                                }
                                AllApps.listOfApps.Add(new finalAppItem
                                {
                                    appName = applist.DisplayInfo.DisplayName,
                                    appFullName = item.Id.FullName,
                                    appDeveloper = item.Id.Publisher,
                                    appInstalled = item.InstalledDate,
                                    appLogo = temp
                                });
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                Crashes.TrackError(e);
                                Analytics.TrackEvent(e.ToString());
                            }
                        });
                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine(e.Message);
                        Crashes.TrackError(e);
                    }
                }
                GlobalVariables.AppForeground = AllApps.listOfApps[0].ForegroundColor;
                GlobalVariables.AppForeGroundOpacity = AllApps.listOfApps[0].ForegroundOpacity;
                GlobalVariables.AppBackground = AllApps.listOfApps[0].BackgroundColor;
                GlobalVariables.AppBackgroundOpacity = AllApps.listOfApps[0].BackgroundOpacity;
                AppsRetreived?.Invoke(true, EventArgs.Empty);
            }
        }
   }
}