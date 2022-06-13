using appLauncher.Core.Brushes;
using appLauncher.Helpers;

using Microsoft.AppCenter.Crashes;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace appLauncher.Model
{
    /// <summary>
    /// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
    /// </summary>
    public class finalAppItem
    {
        public finalAppItem()
        { }
        private string _appfullname;
        public finalAppItem(AppListEntry app, Package pack)
        {
            this.AppListentry = app;
            this.Pack = pack;
            Task a = SetLogo();
            a.Wait();
        }
        [JsonIgnore]
        public AppListEntry AppListentry { get; set; }
        [JsonIgnore]
        public Package Pack { get; set; }
        [JsonIgnore]
        public string appName => this.AppListentry.DisplayInfo.DisplayName;
        [JsonIgnore]
        public string appDeveloper => this.Pack.Id.Publisher;
        [JsonIgnore]
        public DateTimeOffset appInstalled => this.Pack.InstalledDate;
        public string appFullName
        {
            set { _appfullname = value; }
            get { return this.Pack.Id.FullName; }
        }
        [JsonIgnore]
        private byte[] appLogo { get; set; }
        public Color ForegroundColor { get; set; } = Colors.Red;
        public Color BackgroundColor { get; set; } = Colors.LightGreen;
        public double AppTileForegroundOpacity { get; set; } = 1;
        public double AppTileBackgroundOpacity { get; set; } = .50;
        public SolidColorBrush TextBrush()
        {
            return new SolidColorBrush
            {
                Color = this.ForegroundColor,
                Opacity = AppTileForegroundOpacity
            };
        }
        public MaskedBrush LabelBrush()
        {
            try
            {
                return new MaskedBrush(this.appLogo, this.ForegroundColor);
            }
            catch (Exception e)
            {

                Crashes.TrackError(e);
            }
            return null;
        }
        public SolidColorBrush BackgroundBrush()
        {
            return new SolidColorBrush
            {
                Color = this.BackgroundColor,
                Opacity = this.AppTileBackgroundOpacity
            };
        }
        public async Task SetLogo()
        {
            RandomAccessStreamReference logoStream = this.AppListentry.DisplayInfo.GetLogo(new Size(50, 50));
            IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
            byte[] temp = new byte[whatIWant.Size];
            using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
            {
                await read.LoadAsync((uint)whatIWant.Size);
                read.ReadBytes(temp);
            }
            this.appLogo = temp;
        }
        public async Task<bool> LaunchAsync()
        {
            return await this.AppListentry.LaunchAsync();
        }

    }


    public static class AllApps
    {
        public static PaginationObservableCollection<finalAppItem> listOfApps { get; set; }
        /// <summary>
        /// Gets installed apps from device and stores them in an ObservableCollection of finalAppItem, which can be accessed from anywhere.
        /// </summary>
        /// <returns></returns>
        public static async Task getApps()
        {
            await PackageHelper.GetAllAppsAsync();
        }


    }
}
