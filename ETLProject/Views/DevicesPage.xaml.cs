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
    /// Klasa widoku produktów.
    /// Zawiera inicjalizację navigationHelper'a by obsłużyć przycisk wstecz
    /// Obiekt ViewModel klasy DevicesViewModel pozwala odnosić się do "modelu widoku" tej ramki.
    /// </summary>
    public sealed partial class DevicesPage : Page
    {
        private NavigationHelper navigationHelper;

        DevicesViewModel viewModel;

        DateTime timestamp = DateTime.MinValue;
        /// <summary>
        /// Navigation helper obsługujący nawigację po aplikacji.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        /// <summary>
        /// Kontstruktor widoku.
        /// </summary>
        public DevicesPage()
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
        /// Domyślna funkcja NavigationHelper generowana przez Visual Studio. Poniższy opis bez zmian.
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
        /// Metoda NavigationHelpera, jednak dostosowana do potrzeb aplikacji.
        /// Najpierw sprawdzane są argumenty przesłane do widoku (np. ostatni widok).
        /// Następnym krokiem jest weryfikacja stempla czasowego i załadowanie danych metodą LoadData().
        /// Stempel potrzebny by ustalić czy dane mają być załadowane.
        /// Po załadowaniu danych wywoływana jest metoda ListTextChangeVisibilityAfterRefresh(). 
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            if (viewModel != null && timestamp >= viewModel.Timestamp)
                return;

            LoadData();

            ListTextChangeVisibilityAfterRefresh();


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

        /// <summary>
        /// Metoda ładująca dane do listy Produktów.
        /// By poprawnie wyświetlić dane ładowany jest ViewModel napisany specjalnie dla tego widoku.
        /// ViewModel jest potrzebny, by poprawnie załadować dane z bazy SQLite.
        /// Następnie pobrane dane z ViewModel'u ładowane są do kontrolki ListView i prezentowane na odpowiedniej stronie aplikacji.
        /// </summary>
        private void LoadData()
        {
            viewModel = DevicesViewModel.GetDefault();
            DevicesListView.ItemsSource = viewModel.GetAllItems();
        }

        /// <summary>
        /// Metoda mająca na celu sprawdzenie, czy na liście istnieją jakieś elementy.
        /// Jeżeli nie przeprowadzono procesu ETL zamiast listy, wyświetlany jest stosowny komunikat.
        /// </summary>
        private void ListTextChangeVisibilityAfterRefresh()
        {
            if (DevicesListView.Items.Count == 0)
            {
                DevicesListView.Visibility = Visibility.Collapsed;
                EmptyDatabase.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Metoda wywoływana w czasie wyboru przez użytkownika produktu załadowanego z bazy SQLite.
        /// Ma za zadanie wywoływać nowy widok CommentsPage. Przesyła argument do widoku, którym jest ID produktu z bazy SQLite.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesListView.SelectedItem != null)
            {
                Frame.Navigate(typeof(CommentsPage), ((Device)DevicesListView.SelectedItem).Id);
            }
        }

        /// <summary>
        /// Metoda wywoływana w czasie wybrania opcji "Wyczyść dane".
        /// Ma na celu czyszczenie bazy danych ze wszystkich pobranych danych.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
