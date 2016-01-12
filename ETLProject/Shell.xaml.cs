using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ETLProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Shell(Frame frame)
        {
            this.InitializeComponent();
            this.ShellSplitView.Content = frame;

            //code to update menu selected on backpress
            var update = new Action(() =>
            {

                // as long as your buttons are named the same as the page name with "Button" (i.e. HomePageButton) at the end this will work
                if (((Frame)ShellSplitView.Content).SourcePageType.Name.ToString() != "CommentsPage") { 
                var umb = (RadioButton)this.FindName(((Frame)ShellSplitView.Content).SourcePageType.Name.ToString() + "Button");
                umb.IsChecked = true;
                }

            });

            frame.Navigated += (s, e) => update();
            this.Loaded += (s, e) => update();
            this.DataContext = this;


        }

        private void OnHamburgerMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            //((RadioButton)sender).IsChecked = false;
        }

        private void OnHomeButtonChecked(object sender, RoutedEventArgs e)
        {
            
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Views.MainPage));
        }

        private void OnSearchButtonChecked(object sender, RoutedEventArgs e)
        {
            
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Views.DevicesPage));
        }

        //private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
