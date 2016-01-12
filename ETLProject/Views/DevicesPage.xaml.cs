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
using ETLProject.Models;
using ETLProject.ViewModels;
using ETLProject.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ETLProject.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevicesPage : Page
    {
        private NavigationHelper navigationHelper;

        DevicesViewModel viewModel;

        DateTime timestamp = DateTime.MinValue;

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public DevicesPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;


        }


        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            if (viewModel != null && timestamp >= viewModel.Timestamp)
                return;

            LoadData();

            ListTextChangeVisibilityAfterRefresh();


            timestamp = DateTime.Now;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void LoadData()
        {
            viewModel = DevicesViewModel.GetDefault();
            DevicesListView.ItemsSource = viewModel.GetAllItems();
        }


        private void ListTextChangeVisibilityAfterRefresh()
        {
            if (DevicesListView.Items.Count == 0)
            {
                DevicesListView.Visibility = Visibility.Collapsed;
                EmptyDatabase.Visibility = Visibility.Visible;
            }
        }



        private void DevicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView.SelectedItem != null)
            {
                Frame.Navigate(typeof(CommentsPage), ((Device)DevicesListView.SelectedItem).Id);
            }
        }

        private async void ResetDataButton_Click(object sender, RoutedEventArgs e)
        {

            ResetDataButton.IsEnabled = false;

            DevicesListView.ItemsSource = null;
            await ETLProject.CreateDatabase.ResetDataAsync(App.conn);
            LoadData();

            ResetDataButton.IsEnabled = true;

            ListTextChangeVisibilityAfterRefresh();
        }
    }
}
