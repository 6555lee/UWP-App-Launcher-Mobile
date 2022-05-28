﻿using appLauncher.Model;
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
using appLauncher.Helpers;
using Windows.UI;
using System.Threading.Tasks;

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

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                if (await GlobalVariables.IsFilePresent("images.txt"))
                {
                    GlobalVariables.backgroundImage.Add(new BackgroundImages
                    {
                        BitmapImageImage = await GlobalVariables.ReturnImage(file),
                        Filename = file.DisplayName,
                        ImageData = await GlobalVariables.ConvertImageFiletoByteArrayAsync(file),
                    });


                }
                else
                {
                    GlobalVariables.backgroundImage.Add(new BackgroundImages
                    {
                        BitmapImageImage = await GlobalVariables.ReturnImage(file),
                        Filename = file.DisplayName,
                        ImageData = await GlobalVariables.ConvertImageFiletoByteArrayAsync(file),
                    });

                }
            }

            else
            {
                GlobalVariables.bgimagesavailable = false;
            }
            
        }
    


       private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

           

        }


        private BackgroundImages selectedimage { get; set; }
     


        private void imagelist_ItemClick(object sender, ItemClickEventArgs e)
        {
            
           
           
           
        }

      

  

        private void RemoveButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (imagelist.SelectedIndex == -1)
            {
                return;
            }
            BackgroundImages bi = (BackgroundImages)imagelist.SelectedItem;
            if (GlobalVariables.backgroundImage.Any(x => x.Filename == bi.Filename))
            {
                var files = (from x in GlobalVariables.backgroundImage where x.Filename == bi.Filename select x).ToList();
                foreach (var item in files)
                {
                    GlobalVariables.backgroundImage.Remove(item);
                }
            }
        }
    }
    } 

