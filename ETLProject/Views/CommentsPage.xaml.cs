using ETLProject.Common;
using ETLProject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ETLProject.Views
{
    /// <summary>
    /// Klasa widoku komentarzy.
    /// Zawiera inicjalizację navigationHelper'a by obsłużyć przycisk wstecz
    /// Obiekt ViewModel klasy CommentsViewModel pozwala odnosić się do "modelu widoku" tej ramki.
    /// </summary>
    public sealed partial class CommentsPage : Page
    {
        private NavigationHelper navigationHelper;

        CommentsViewModel viewModel;

        DateTime timestamp = DateTime.MinValue;

        /// <summary>
        /// Navigation helper obsługujący nawigację po aplikacji.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Konstruktor widoku.
        /// </summary>
        public CommentsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Domyślna metoda NavigationHelper generowana przez Visual Studio. Poniższy opis bez zmian.
        /// 
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
        /// Domyślna metoda NavigationHelper generowana przez Visual Studio. Poniższy opis bez zmian.
        /// 
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


        /// <summary>
        /// Metoda NavigationHelper'a. Pobiera dane przekazywane z widoku źródłowego (DevicesPage).
        /// deviceId jest argumentem przekazywanym z widoku Produktów.
        /// Następnie ładowany jest ViewModel DevicesViewModel by pobrać nazwę urządzenia (w celu wyświetlenia jako nagłówek strony)
        /// Komentarze dla danego produkty są pobierane z bazy danych oraz przypisywane do listy i wyświetlane.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);



            var deviceID = (long)e.Parameter;

            var commViewModel = DevicesViewModel.GetDefault();
            pageTitle.Text = commViewModel.GetItem(deviceID).Name;

            if (viewModel != null && (deviceID == viewModel.DeviceId) && (timestamp >= viewModel.Timestamp))
                return;

            viewModel = CommentsViewModel.GetForDeviceId(deviceID);
            CommentsList.ItemsSource = viewModel.GetAllItems();

            timestamp = DateTime.Now;
        }
        /// <summary>
        /// Domyślna metoda navigationHelper'a. Przesyła argumenty do innych widoków.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}

