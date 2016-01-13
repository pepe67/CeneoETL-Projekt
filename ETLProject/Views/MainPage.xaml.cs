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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ETLProject.Views
{
    /// <summary>
    /// Klasa widoku MainPage, w którym umieszczone są kontrolki wyszukiwania i procesu ETL.
    /// Zawiera jedynie kontstruktor z inicjaliazcją.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Konstruktor klasy Main Page
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            
        }
    }
}
