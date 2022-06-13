using appLauncher.Animations;
using appLauncher.Control;
using appLauncher.Helpers;
using appLauncher.Model;
using appLauncher.Pages;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Toolkit.Uwp.UI.Animations;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace appLauncher
{

    /// <summary>
    /// The page where the apps are displayed. Most of the user interactions with the app launcher will be here.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int maxRows;
        private int maxColumns;
        public static FlipViewItem flipViewTemplate;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        // Delays updating the app list when the size changes.
        DispatcherTimer sizeChangeTimer = new DispatcherTimer();
        int currentTimeLeft = 0;
        const int updateTimerLength = 100; // milliseconds;
        TimeSpan timeSpan = TimeSpan.FromSeconds(15);
        DispatcherTimer dispatching = new DispatcherTimer();
        int currentindex = 0;



        /// <summary>
        /// Runs when a new instance of MainPage is created
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            sizeChangeTimer.Tick += SizeChangeTimer_Tick;
            screensContainerFlipView.Items.VectorChanged += Items_VectorChanged;
            backimage.RotationDelay = timeSpan;
        }

        internal async void UpdateIndicator(int pagenum)
        {
            await AdjustIndicatorStackPanel(pagenum);
        }

        // Updates grid of apps only when a bit of time has passed after changing the size of the window.
        // Better than doing this inside the the flip view item template since you don't have a timer that's always running anymore.
        private void SizeChangeTimer_Tick(object sender, object e)
        {
            AllApps.listOfApps.CurrentPage = this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;
            if (currentTimeLeft == 0)
            {
                currentTimeLeft = 0;
                sizeChangeTimer.Stop();
                maxRows = GlobalVariables.NumofRoworColumn(12, 84, (int)screensContainerFlipView.ActualHeight);
                maxColumns = GlobalVariables.NumofRoworColumn(12, 64, (int)screensContainerFlipView.ActualWidth);
                GlobalVariables.columns = maxColumns;
                AllApps.listOfApps.PageSize = GlobalVariables.appsperscreen = maxColumns * maxRows;
                int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
                int fullPages = additionalPagesToMake;
                int appsLeftToAdd = AllApps.listOfApps.Count() - (fullPages * GlobalVariables.appsperscreen);
                if (appsLeftToAdd > 0)
                {
                    additionalPagesToMake += 1;
                }
                if (additionalPagesToMake > 0)
                {
                    screensContainerFlipView.Items.Clear();
                    for (int i = 0; i < additionalPagesToMake; i++)
                    {
                        screensContainerFlipView.Items.Add(i);
                    }
                }

                this.InvalidateArrange();

            }
            else
            {
                currentTimeLeft -= (int)sizeChangeTimer.Interval.TotalMilliseconds;
            }

        }

        internal object getFlipview()
        {
            return screensContainerFlipView;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!sizeChangeTimer.IsEnabled)
            {
                sizeChangeTimer.Interval = TimeSpan.FromMilliseconds(updateTimerLength / 10);
                sizeChangeTimer.Start();
            }
            currentTimeLeft = updateTimerLength;

        }

        private async void Items_VectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            var collection = sender;
            int count = collection.Count;

            flipViewIndicatorStackPanel.Children.Clear();

            for (int i = 0; i < count; i++)
            {
                flipViewIndicatorStackPanel.Children.Add(new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(4, 0, 4, 0)
                });

            };
            await AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await this.Scale(2f, 2f, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 0).StartAsync();
            await this.Scale(1, 1, (float)this.ActualWidth / 2, (float)this.ActualHeight / 2, 300).StartAsync();
        }



        /// <summary>
        /// Runs when the page has loaded
        /// <para> Sorts all of the apps into pages based on how
        /// based on the size of the app window/device's screen resolution.
        /// </para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GlobalVariables.LoadBackgroundImages();
            AllApps.listOfApps.CurrentPage = this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;
            maxRows = GlobalVariables.NumofRoworColumn(12, 94, (int)screensContainerFlipView.ActualHeight);
            maxColumns = GlobalVariables.NumofRoworColumn(12, 74, (int)screensContainerFlipView.ActualWidth);
            AllApps.listOfApps.PageSize = GlobalVariables.columns = maxColumns;
            GlobalVariables.appsperscreen = maxColumns * maxRows;
            int additionalPagesToMake = calculateExtraPages(GlobalVariables.appsperscreen) - 1;
            int fullPages = additionalPagesToMake;
            int appsLeftToAdd = AllApps.listOfApps.Count() - (fullPages * GlobalVariables.appsperscreen);
            if (appsLeftToAdd > 0)
            {
                additionalPagesToMake += 1;
            }
            //NOTE: I wasn't able to create an ItemTemplate from C# so I made a GridView
            //in the XAML view with the desired values and used its 
            //item template to create identical GridViews.

            //If you know how to create ItemTemplates in C#, please make a pull request which
            //with a new solution for this issue or contanct me directly. It would make things way easier for everyone!
            //  DataTemplate theTemplate = appGridView.ItemTemplate;


            //Following code creates any extra app pages then adds apps to each page.
            if (additionalPagesToMake > 0)
            {


                for (int i = 0; i < additionalPagesToMake; i++)
                {

                    screensContainerFlipView.Items.Add(i);
                }

                screensContainerFlipView.SelectionChanged += FlipViewMain_SelectionChanged;
            }
            this.screensContainerFlipView.SelectedIndex = (GlobalVariables.pagenum > 0) ? GlobalVariables.pagenum : 0;

            await AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        /// <summary>
        /// Attempts to disable vertical scrolling.
        /// </summary>
        /// <param name="gridView"></param>
        private void disableScrollViewer(GridView gridView)
        {
            try
            {
                var border = (Border)VisualTreeHelper.GetChild(gridView, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.IsVerticalRailEnabled = false;
                scrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Crashes.TrackError(e);
            }
        }

        /// <summary>
        /// Returns result of calculation of extra pages needed to be added.
        /// </summary>
        /// <param name="appsPerScreen"></param>
        /// <returns></returns>
        private int calculateExtraPages(int appsPerScreen)
        {
            double appsPerScreenAsDouble = appsPerScreen;
            double numberOfApps = AllApps.listOfApps.Count();
            int pagesToMake = (int)Math.Ceiling(numberOfApps / appsPerScreenAsDouble);
            return pagesToMake;
        }

        /// <summary>
        /// Runs when launcher settings is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Analytics.TrackEvent("Navigating from main page to settings page");
            Debug.WriteLine("You clicked on the settings icon");
            Frame.Navigate(typeof(settings));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns></returns>
        private async Task AdjustIndicatorStackPanel(int selectedIndex)
        {
            var indicator = flipViewIndicatorStackPanel;
            Ellipse ellipseToAnimate = new Ellipse();
            for (int i = 0; i < indicator.Children.Count; i++)
            {
                if (i == selectedIndex)
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipseToAnimate = ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    var ellipse = (Ellipse)indicator.Children[i];
                    ellipse.Fill = (SolidColorBrush)App.Current.Resources["DefaultTextForegroundThemeBrush"];
                }
            }
            float centerX = (float)ellipseToAnimate.ActualWidth / 2;
            float centerY = (float)ellipseToAnimate.ActualHeight / 2;
            float animationScale = 1.7f;
            double duration = 300;

            if (IndicatorAnimation.oldAnimatedEllipse != null)
            {
                await Task.WhenAll(ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Back).StartAsync(),
                IndicatorAnimation.oldAnimatedEllipse.Scale(1, 1, centerX, centerY, duration, easingType: EasingType.Back).StartAsync());
            }
            else
            {
                await ellipseToAnimate.Scale(animationScale, animationScale, centerX, centerY, duration, easingType: EasingType.Bounce).StartAsync();
            }

            IndicatorAnimation.oldAnimatedEllipse = ellipseToAnimate;
        }



        private void allAppsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchPage));
        }

        private void Filterby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = ((ComboBoxItem)Filterby.SelectedItem).Content.ToString();
            switch (selected)
            {
                case "AtoZ":
                    {

                        var reordered = AllApps.listOfApps.OrderBy(x => x.appName).ToList();
                        AllApps.listOfApps = new PaginationObservableCollection<finalAppItem>(reordered);
                    }

                    break;
                case "Developer":
                    {

                        var reordered = AllApps.listOfApps.OrderBy(x => x.appDeveloper).ToList();
                        AllApps.listOfApps = new PaginationObservableCollection<finalAppItem>(reordered);
                    }
                    break;
                case "Installed":
                    {
                        var reordered = AllApps.listOfApps.OrderBy(x => x.appInstalled).ToList();
                        AllApps.listOfApps = new PaginationObservableCollection<finalAppItem>(reordered);
                    }
                    break;
                default:
                    break;
            }
            this.Frame.Navigate(typeof(appLauncher.MainPage));
        }
        private async void FlipViewMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalVariables.pagenum = ((FlipView)sender).SelectedIndex;
            if (e.AddedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromIndex(screensContainerFlipView.SelectedIndex);
                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedToThisPage();
            }
            if (e.RemovedItems.Count > 0)
            {
                var flipViewItem = screensContainerFlipView.ContainerFromItem(e.RemovedItems[0]);
                appControl userControl = FindFirstElementInVisualTree<appControl>(flipViewItem);
                userControl.SwitchedFromThisPage();
            }
            await AdjustIndicatorStackPanel(GlobalVariables.pagenum);
        }

        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;

                }
            }
            return null;
        }


    }
}
