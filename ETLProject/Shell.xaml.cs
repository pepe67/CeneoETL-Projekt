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



namespace ETLProject
{
    /// <summary>
    /// /// Projekt aplikacji ETL oparty na środowisku Windows 10 UWP.
    /// Aplikacja działa zarówno na PC, tabletach jak i telefonach z systemem Windows 10.
    /// Jest również dostępna do pobrania ze sklepu: https://www.microsoft.com/pl-pl/store/apps/ceneoetl/9nblggh5jqp6
    /// 
    /// Klasa widoku Shell. Jest to główne okno aplikacji oparte o kontrolkę SplitView.
    /// </summary>
    public sealed partial class Shell : Page
    {
        /// <summary>
        /// Konstruktor i inicjalizacja widoku Shell
        /// </summary>
        /// <param name="frame"></param>
        public Shell(Frame frame)
        {
            this.InitializeComponent();
            this.ShellSplitView.Content = frame;

            //code to update menu selected on backpress
            var update = new Action(() =>
            {

                // Zmiana stanu kontrolek po wciśnięciu przycisku wstecz.
                if (((Frame)ShellSplitView.Content).SourcePageType.Name.ToString() != "CommentsPage") { 
                var umb = (RadioButton)this.FindName(((Frame)ShellSplitView.Content).SourcePageType.Name.ToString() + "Button");
                umb.IsChecked = true;
                }

            });

            frame.Navigated += (s, e) => update();
            this.Loaded += (s, e) => update();
            this.DataContext = this;


        }
        /// <summary>
        /// Metoda otwierająca i zamykające "Hamburger" menu aplikacji.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHamburgerMenuButtonClicked(object sender, RoutedEventArgs e)
        {
            ShellSplitView.IsPaneOpen = !ShellSplitView.IsPaneOpen;
            //((RadioButton)sender).IsChecked = false;
        }
        /// <summary>
        /// Kliknięcie opcji "Wyszukiwanie" w menu aplikacji wywołuje tą metodę, która ma za zadanie załadować stronę
        /// wyszukiwania i procesu ETL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHomeButtonChecked(object sender, RoutedEventArgs e)
        {
            
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Views.MainPage));
        }
        /// <summary>
        /// Metoda wywoływana po wybraniu opcji Przeglądanie Wyników ETL w menu aplikacji.
        /// Ma za zadanie załadowanie widoku przeglądania danych zapisanych w bazie SQLite w pamięci urządzenia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearchButtonChecked(object sender, RoutedEventArgs e)
        {
            
            if (ShellSplitView.Content != null)
                ((Frame)ShellSplitView.Content).Navigate(typeof(Views.DevicesPage));
        }

    }
}
