using appLauncher.Core;
using appLauncher.Model;

using Microsoft.AppCenter.Analytics;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        
        public SearchPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += SearchPage_BackRequested;
            DesktopBackButton.ShowBackButton();
            QueriedAppsListView.ItemsSource = AllApps.listOfApps;
            Analytics.TrackEvent("Search Page loaded");
        }

        private void SearchPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            DesktopBackButton.HideBackButton();
            e.Handled = true;
            Frame.Navigate(typeof(MainPage));
        }

        private void useMeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = useMeTextBox.Text.ToLower();
            if (!String.IsNullOrEmpty(query))
            {
                QueriedAppsListView.ItemsSource = AllApps.listOfApps.Where(p => p.appName.ToLower().Contains(query));

            }
            else
            {
                QueriedAppsListView.ItemsSource = AllApps.listOfApps;
            }
           
        }
        private async void QueriedAppsListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            await ((finalAppItem)e.ClickedItem).Launch();


        }


    }
}
