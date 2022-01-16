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

namespace appLauncher.Model
{
	/// <summary>
	/// A class made up of the app list entry and the app logo of each app. This is what each app control displayed represents.
	/// </summary>
	public class finalAppItem
	{
		public string appName { get; set; }
		public string appDeveloper { get; set; }
		public DateTimeOffset appInstalled { get; set; }
		public  string appFullName { get; set; }
		public Byte[] appLogo { get; set; }
		public Color ForegroundColor { get; set; } = Colors.Red;
		public Color BackgroundColor { get; set; } = Colors.Black;
		public Double ForegroundOpacity { get; set; } = .90;
		public Double BackgroundOpacity { get; set; } = .35;
		public SolidColorBrush TextBrush()
        {
			return new SolidColorBrush
			{
				Color = this.ForegroundColor,
				Opacity = this.ForegroundOpacity
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
				Opacity = this.BackgroundOpacity
			};
        }
		public async Task Launch()
        {
			try
			{
				PackageManager pkgmgr = new PackageManager();
				Package applist = pkgmgr.FindPackage(this.appFullName);
				var aplistentry = await applist.GetAppListEntriesAsync();
				await aplistentry[0].LaunchAsync();
			}
			catch (Exception e)
            {
				Debug.WriteLine(e.Message);
				Crashes.TrackError(e);
            }
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
