using appLauncher.Helpers;
using appLauncher.Model;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236


namespace appLauncher.Control
{
    public sealed partial class appControl : UserControl
    {
        int page;
        DispatcherTimer dispatcher;

        //public ObservableCollection<finalAppItem> listOfApps
        //{
        //    get { return (ObservableCollection<finalAppItem>)GetValue(listOfAppsProperty); }
        //    set { SetValue(listOfAppsProperty, value); }
        //}
        //// Using a DependencyProperty as the backing store for listOfApps.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty listOfAppsProperty =
        //    DependencyProperty.Register("listOfApps", typeof(ObservableCollection<finalAppItem>), typeof(appControl), null);
        public appControl()
        {
            this.InitializeComponent();
            this.Loaded += AppControl_Loaded;
        }

        private void AppControl_Loaded(object sender, RoutedEventArgs e)
        {
            AllApps.listOfApps.CurrentPage = GlobalVariables.pagenum;
            AllApps.listOfApps.PageSize = GlobalVariables.appsperscreen;
            GridViewMain.ItemsSource = AllApps.listOfApps;
        }

        private void Dispatcher_Tick(object sender, object e)
        {
            ProgressRing.IsActive = false;
            dispatcher.Stop();
            AllApps.listOfApps.CurrentPage = GlobalVariables.pagenum;
            AllApps.listOfApps.PageSize = GlobalVariables.appsperscreen;
            GridViewMain.ItemsSource = AllApps.listOfApps;
        }
        public void SwitchedToThisPage()
        {
            AllApps.listOfApps.CurrentPage = GlobalVariables.pagenum;
            AllApps.listOfApps.PageSize = GlobalVariables.appsperscreen;
            GridViewMain.ItemsSource = AllApps.listOfApps;
        }

        public void SwitchedFromThisPage()
        {
            GridViewMain.ItemsSource = null;
        }

        private void GridViewMain_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            GlobalVariables.isdragging = true;
            object item = e.Items.First();
            var source = sender;
            e.Data.Properties.Add("item", item);
            GlobalVariables.itemdragged = (finalAppItem)item;
            GlobalVariables.oldindex = AllApps.listOfApps.IndexOf((finalAppItem)item);
        }

        private void GridViewMain_Drop(object sender, DragEventArgs e)
        {
            GridView view = sender as GridView;

            //Find the position where item will be dropped in the gridview
            Point pos = e.GetPosition(view.ItemsPanelRoot);

            //Get the size of one of the list items
            GridViewItem gvi = (GridViewItem)view.ContainerFromIndex(0);
            double itemHeight = gvi.ActualHeight + gvi.Margin.Top + gvi.Margin.Bottom;
            double itemwidth = gvi.ActualHeight + gvi.Margin.Left + gvi.Margin.Right;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(view.Items.Count - 1, (int)(pos.Y / itemHeight));
            int indexy = Math.Min(view.Items.Count - 1, (int)(pos.X / itemwidth));
            var t = (List<finalAppItem>)view.ItemsSource;
            var te = t[((index * GlobalVariables.columns) + (indexy))];
            GlobalVariables.newindex = AllApps.listOfApps.IndexOf(te);
            AllApps.listOfApps.RemoveAt(GlobalVariables.oldindex);
            AllApps.listOfApps.Insert(GlobalVariables.newindex, GlobalVariables.itemdragged);
            GlobalVariables.pagenum = (int)this.DataContext;
            SwitchedToThisPage();
            ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(GlobalVariables.pagenum);

        }

        private void GridViewMain_DragOver(object sender, DragEventArgs e)
        {
            GridView d = (GridView)sender;
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            FlipView c = (FlipView)((Window.Current.Content as Frame).Content as MainPage).getFlipview();
            Point startpoint = e.GetPosition(this);
            if (GlobalVariables.startingpoint.X == 0)
            {
                GlobalVariables.startingpoint = startpoint;
            }
            else
            {
                var a = this.TransformToVisual(c);
                var b = a.TransformPoint(new Point(0, 0));
                if (GlobalVariables.startingpoint.X > startpoint.X && startpoint.X < (b.X + 100))
                {
                    if (c.SelectedIndex > 0)
                    {
                        c.SelectedIndex -= 1;
                        GlobalVariables.startingpoint = startpoint;
                    }
                }
                else if (GlobalVariables.startingpoint.X < startpoint.X && startpoint.X > (b.X + d.ActualWidth - 100))
                {
                    if (c.SelectedIndex < c.Items.Count() - 1)
                    {
                        c.SelectedIndex += 1;
                        GlobalVariables.startingpoint = startpoint;
                    }

                }
            }
            GlobalVariables.pagenum = c.SelectedIndex;
            ((Window.Current.Content as Frame).Content as MainPage).UpdateIndicator(GlobalVariables.pagenum);
        }

        private async void GridViewMain_ItemClick(object sender, ItemClickEventArgs e)
        {
            finalAppItem fi = (finalAppItem)e.ClickedItem;
            await fi.LaunchAsync();
        }


    }
}
