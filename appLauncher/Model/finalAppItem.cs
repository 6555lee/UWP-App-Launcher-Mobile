using appLauncher.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Composition;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Windows.UI.Xaml;
using System.Numerics;
using appLauncher.Core.Brushes;
using Windows.Management.Deployment;
using Swordfish.NET.Collections;
using Microsoft.AppCenter.Crashes;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.Foundation;

namespace appLauncher.Model
{
	/// <summary>
	/// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
	/// </summary>
	public class finalAppItem
	{
		public finalAppItem()
		{ }
		public finalAppItem(AppListEntry app,Package pack, byte[] bytes)
        {
			this.appName = app.DisplayInfo.DisplayName;
			this.appFullName = pack.Id.FullName;
			this.appDeveloper = pack.Id.Publisher;
			this.appInstalled = pack.InstalledDate;
			this.appLogo = bytes;
        }
		[JsonIgnore]
		public AppListEntry AppListentry { get; set; }
		[JsonIgnore]
		public Package Pack { get; set; }
		public string appName { get; set; }
		public string appDeveloper { get; set; }
		public DateTimeOffset appInstalled { get; set; }
		public  string appFullName { get; set; }
		public byte[] appLogo { get; set; }
		public Color ForegroundColor { get; set; } = Colors.Red;
		public Color BackgroundColor { get; set; } = Colors.LightGreen;
		public double AppTileForegroundOpacity { get; set; } = 1;
		public double AppTileBackgroundOpacity { get; set; } = .50;
		public SolidColorBrush TextBrush()
        {
			return new SolidColorBrush
			{
				Color = this.ForegroundColor,
				Opacity = Convert.ToDouble(this.ForegroundColor.A)
			};
        }
		public MaskedBrush LabelBrush()
        {
			return new MaskedBrush(this.appLogo, this.ForegroundColor);
		}
		public SolidColorBrush BackgroundBrush()
        {
			return new SolidColorBrush
			{
				Color = this.BackgroundColor,
				Opacity =Convert.ToDouble(this.BackgroundColor.A)
			};
        }
		//public async Task SetLogo()
		//{
		//	var logoStream = this.AppListentry.DisplayInfo.GetLogo(new Size(50, 50));
		//	IRandomAccessStreamWithContentType whatIWant = await logoStream.OpenReadAsync();
		//	byte[] temp = new byte[whatIWant.Size];
		//	using (DataReader read = new DataReader(whatIWant.GetInputStreamAt(0)))
		//	{
		//		await read.LoadAsync((uint)whatIWant.Size);
		//		read.ReadBytes(temp);
		//	}
		//	this.appLogo = temp;
		//}
		public async Task<bool> LaunchAsync()
		{
			return await this.AppListentry.LaunchAsync();
		}

	}


	public static class AllApps
	{
		public static ConcurrentObservableCollection<finalAppItem> listOfApps { get; set; } = new ConcurrentObservableCollection<finalAppItem>();
	
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
