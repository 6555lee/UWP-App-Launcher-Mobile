using appLauncher.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace appLauncher.Pages
{
    /// <summary>
    /// Page where the launcher settings are configured
    /// </summary>
    public sealed partial class settings : Page
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        public settings()
        {
            this.InitializeComponent();
            Analytics.TrackEvent("Settings Page Loaded");
        }

        /// <summary>
        /// Runs when the app has navigated to this page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (GlobalVariables.bgimagesavailable)
            {
                imageButton.Content = "Add Image";
            }
        }

        /// <summary>
        /// Launches the file picker and allows the user to pick an image from their pictures library.<br/>
        /// The image will then be used as the background image in the main launcher page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void imageButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            //Standard Image Support
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpe");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".svg");
            picker.FileTypeFilter.Add(".tif");
            picker.FileTypeFilter.Add(".tiff");
            picker.FileTypeFilter.Add(".bmp");

            //JFIF Support
            picker.FileTypeFilter.Add(".jif");
            picker.FileTypeFilter.Add(".jfif");

            //GIF Support
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".gifv");

            var file = await picker.PickMultipleFilesAsync();
            if (file.Any())
            {
              if (await GlobalVariables.IsFilePresent("images.txt"))
                {
                foreach (StorageFile item in file)
                    {

                        byte[] fileBytes = null;
                        using (var stream = await item.OpenReadAsync())
                        {
                            fileBytes = new byte[stream.Size];
                            using (var reader = new DataReader(stream))
                            {
                                await reader.LoadAsync((uint)stream.Size);
                                reader.ReadBytes(fileBytes);
                            }
                            GlobalVariables.backgroundImage.Add(new BackgroundImages
                            {
                                bitmapImage = fileBytes,
                                filename = item.DisplayName,
                            });

                        }
                    }
                }
                else
                {
                    foreach (var item in file)
                    {
                        byte[] fileBytes = null;
                        using (var stream = await item.OpenReadAsync())
                        {
                            fileBytes = new byte[stream.Size];
                            using (var reader = new DataReader(stream))
                            {
                                await reader.LoadAsync((uint)stream.Size);
                                reader.ReadBytes(fileBytes);
                            }
                            GlobalVariables.backgroundImage.Add(new BackgroundImages
                            {
                                bitmapImage = fileBytes,
                                filename = item.DisplayName
                            });
                        }


                    }
                    GlobalVariables.bgimagesavailable = true;
                }
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
            }

      


            private async void RemoveButton_ClickAsync(object sender, RoutedEventArgs e)
            {

                if (imagelist.SelectedIndex != -1)
                {
                    BackgroundImages bi = (BackgroundImages)imagelist.SelectedItem;
                    if (GlobalVariables.backgroundImage.Any(x => x.filename == bi.filename))
                    {
                        var files = (from x in GlobalVariables.backgroundImage where x.filename == bi.filename select x).ToList();
                        foreach (var item in files)
                        {
                            GlobalVariables.backgroundImage.Remove(item);
                        }
                    }
                }

            }



            private void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
            {

            }

            private void ListView_Drop(object sender, DragEventArgs e)
            {

            }

            private void Page_Loaded(object sender, RoutedEventArgs e)
            {
            BackImageColorPicker.Opacity = GlobalVariables.ImageOpacity;
            BackImageColorPicker.Color = GlobalVariables.ImageColor;
            ForegroundColorPicker.Color = GlobalVariables.AppForeground;
            ForegroundColorPicker.Opacity = GlobalVariables.AppForeGroundOpacity;
            BackGroundColorPicker.Color = GlobalVariables.AppBackground;
            BackGroundColorPicker.Opacity = GlobalVariables.AppBackgroundOpacity;

            }

        private void BackImageColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            foreach (BackgroundImages item in GlobalVariables.backgroundImage)
            {
                item.ImageColor = BackImageColorPicker.Color;
                item.ImageOpacity = BackImageColorPicker.Opacity;                                                             
            }
        }

        private void ForegroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            foreach (finalAppItem item in AllApps.listOfApps)
            {
                item.ForegroundColor = ForegroundColorPicker.Color;
                item.ForegroundOpacity = ForegroundColorPicker.Opacity;
            }
        }

        private void BackGroundColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            foreach (finalAppItem item in AllApps.listOfApps)
            {
                item.BackgroundColor = BackGroundColorPicker.Color;
                item.BackgroundOpacity = BackGroundColorPicker.Opacity;
            }
        }
    }
    } 

