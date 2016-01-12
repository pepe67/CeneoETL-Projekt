using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETLProject.Models;
using ETLProject;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Popups;

namespace ETLProject.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Ceneo> _ceneoProductList = new ObservableCollection<Ceneo>();
        public ObservableCollection<Skapiec> _skapiecProductList = new ObservableCollection<Skapiec>();

        Extract e = new Extract();
        Transform t = new Transform();
        List<Opinia> opinie = new List<Opinia>();
        string[] DeviceTransformedData = new string[3];

        public MainViewModel()
        {
            //AddCeneoProductList("Test Nazwa", "Test URL");
            //AddSkapiecProductList("Test Nazwa", "Test URL");

        }

        public RelayCommand SearchButtonCommand
        {
            get
            {
                //GetDatais a method for your action on click event
                return new RelayCommand(SearchMethodsStart);
            }
        }

        private async void SearchMethodsStart()
        {
            SearchButtonEnabled = false;
            TransformButtonEnabled = false;
            LoadButtonEnabled = false;
            ExtractButtonEnabled = false;
            ETLButtonEnabled = false;
            ProgressActive = true;
            opinie.Clear();
            _ceneoProductList.Clear();
            _skapiecProductList.Clear();
            ResultTextBox = "";
            await CeneoSearchMethod();
            await SkapiecSearchMethod();
            SearchButtonEnabled = true;
            ProgressActive = false;
        }

        public RelayCommand ExtractButtonCommand
        {
            get
            {
                //GetDatais a method for your action on click event
                return new RelayCommand( ExtractStart);
            }
        }

        private async void ExtractStart()
        {
            ProgressActive = true;
            SearchButtonEnabled = false;
            ExtractButtonEnabled = false;
            TransformButtonEnabled = false;
            ETLButtonEnabled = false;
            
            await e.Extraction(_selectedCeneoLista.CeneoUrl, _selectedSkapiecLista.SkapiecUrl);
            ResultTextBox = "\t\t\t CENEO: \n" + e.htmlCeneo + "\n \t\t\t SKAPIEC: " + e.htmlSkapiec;
            //System.Diagnostics.Debug.WriteLine("SUCCESS:" + e.htmlCeneo);
            

            ProgressActive = false;
            SearchButtonEnabled = true;
            
            TransformButtonEnabled = true;
        }

        public RelayCommand TransformButtonCommand
        {
            get
            {
                //GetDatais a method for your action on click event
                return new RelayCommand(TransformStart);
            }
        }

        private async void TransformStart()
        {
            ProgressActive = true;
            SearchButtonEnabled = false;
            ExtractButtonEnabled = false;
            TransformButtonEnabled = false;

            await t.transformation(e.htmlCeneo, e.htmlSkapiec, _selectedCeneoLista.CeneoUrl, _selectedSkapiecLista.SkapiecUrl);
            opinie = t.listaOpinii;
            DeviceTransformedData = t.daneUrzadzenia;
            ResultTextBox = String.Join(Environment.NewLine, opinie.Select(x => x.ToString()));
            System.Diagnostics.Debug.WriteLine("SUCCESS:");

            ProgressActive = false;
            SearchButtonEnabled = true;
            
            LoadButtonEnabled = true;
        }

        public RelayCommand LoadButtonCommand
        {
            get
            {
                //GetDatais a method for your action on click event
                return new RelayCommand(LoadStart);
            }
        }

        private async void LoadStart()
        {
            ProgressActive = true;
            SearchButtonEnabled = false;
            ExtractButtonEnabled = false;
            TransformButtonEnabled = false;
            LoadButtonEnabled = false;
            int licznik = 0;
            Load l = new Load();
     
            foreach (Opinia element in opinie)
            {
                licznik++;
                long deviceId = await CreateDatabase.GetDeviceId(App.conn, DeviceTransformedData[0], DeviceTransformedData[1], DeviceTransformedData[2]);
                if ( deviceId == 0)
                {
                    long insertDeviceId = await l.InsertDevice(DeviceTransformedData[0], DeviceTransformedData[1], DeviceTransformedData[2]);
                    l.InsertCommentForDevice((int)insertDeviceId, element.zalety, element.wady, element.autor, element.podsumowanieOpinii, element.gwiazdki, element.data, element.polecam, element.przydatna, element.pochodzenie);
                }
                else {
                    long commentId = await CreateDatabase.GetCommentId(App.conn, (int)deviceId, element.zalety, element.wady, element.autor, element.podsumowanieOpinii);
                    if (commentId == 0)
                    {
                        l.InsertCommentForDevice((int)deviceId, element.zalety, element.wady, element.autor, element.podsumowanieOpinii, element.gwiazdki, element.data, element.polecam, element.przydatna, element.pochodzenie);
                    }
                    else
                    {
                        licznik--;
                        continue;
                    }
                }
                    
            }
            if (opinie.Count == 0)
            {
                await l.InsertDevice(DeviceTransformedData[0], DeviceTransformedData[1], DeviceTransformedData[2]);
            } 

            ResultTextBox = "Wykonano proces dla urządzenia: " + DeviceTransformedData[0] + ".\n Liczba dodanych komentarzy: " + licznik;

            ProgressActive = false;
            SearchButtonEnabled = true;

        }

        public RelayCommand ETLButtonCommand
        {
            get
            {
                //GetDatais a method for your action on click event
                return new RelayCommand(ETLStart);
            }
        }

        private async void ETLStart()
        {
            ProgressActive = true;
            ETLButtonEnabled = false;
            SearchButtonEnabled = false;
            ExtractButtonEnabled = false;
            TransformButtonEnabled = false;
            LoadButtonEnabled = false;
            
            await e.Extraction(SelectedCeneoLista.CeneoUrl, SelectedSkapiecLista.SkapiecUrl);
            await t.transformation(e.htmlCeneo, e.htmlSkapiec, SelectedCeneoLista.CeneoUrl, SelectedSkapiecLista.SkapiecUrl);

            opinie = t.listaOpinii;
            DeviceTransformedData = t.daneUrzadzenia;

            LoadStart();

            ProgressActive = false;
            SearchButtonEnabled = true;

        }

        async private Task CeneoSearchMethod()
        {
            
            string html;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.ceneo.pl/;szukaj-" + AddresTextBoxText);
            try
            {
                //System.Diagnostics.Debug.WriteLine("SUCCESS:" + URL);
                WebResponse x = await req.GetResponseAsync();
                
                HttpWebResponse res = (HttpWebResponse)x;
                if (res != null)
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = res.GetResponseStream();
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);

                        
                        var productnames = htmlDocument.DocumentNode.DescendantsAndSelf("a").Where(o => o.Attributes["data-reviewshref"] != null);


                        foreach (HtmlNode product in productnames)
                        {
                            string url = product.Attributes["href"].Value;
                            int indexOfSubstring = url.IndexOf("#");
                            url = url.Substring(0, indexOfSubstring);
                            AddCeneoProductList(product.InnerHtml, "http://www.ceneo.pl" + url);

                        }


                    }
                    res.Dispose();
                    
                }
            }
            catch
            {
                //SearchButton.IsEnabled = true;
                MessageDialog messageDialog =
                    new MessageDialog("A tear occured in the space-time continuum. Please try again when all planets in the solar system are aligned (excluding Pluto, which isn't a planet anymore).");
                //Meaningful error messages are important.
                await messageDialog.ShowAsync();
            }
        }

        async private Task SkapiecSearchMethod()
        {

            //ceneoProdukty wybrany = ceneoProduktList[CeneoLista.SelectedIndex];
            //Zwraca nazwę wybranego elementu z listy
            //http://www.skapiec.pl/szukaj/w_calym_serwisie/xbox+360
            //WynikTB.Text = wybrany.CeneoNazwa;

            //SearchButton.IsEnabled = false;
            string html;

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.skapiec.pl/szukaj/w_calym_serwisie/m0r05ea");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.skapiec.pl/szukaj/w_calym_serwisie/" + AddresTextBoxText.Replace(" ", "+"));

            try
            {
                WebResponse x = await req.GetResponseAsync();
                HttpWebResponse res = (HttpWebResponse)x;
                if (res != null)
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = res.GetResponseStream();
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }
                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);

                        var productnames = htmlDocument.DocumentNode.DescendantsAndSelf("a").Where(o => o.GetAttributeValue("class", null) == "redirect");
                        
                        foreach (HtmlNode product in productnames)
                        {
                            if (product.InnerText.Replace("&quot;", "''") == "Idź do sklepu") { }
                            else
                            {
                                AddSkapiecProductList(product.InnerText.Replace("&quot;", "''"), "http://skapiec.pl" + product.GetAttributeValue("href", null));
                            }
                                
                        }
                       

                    }
                    res.Dispose();
                    
                }
            }
            catch
            {
                
                MessageDialog messageDialog =
                    new MessageDialog("A tear occured in the space-time continuum. Please try again when all planets in the solar system are aligned (excluding Pluto, which isn't a planet anymore).");
                //Meaningful error messages are important.
                await messageDialog.ShowAsync();
            }
        }

        private void AddCeneoProductList(String nazwa, string url)
        {
            _ceneoProductList.Add(new Ceneo() { CeneoNazwa = nazwa, CeneoUrl = url });
        }
        private void AddSkapiecProductList(String nazwa, string url)
        {
            _skapiecProductList.Add(new Skapiec() { SkapiecNazwa = nazwa, SkapiecUrl = url });
        }
        public ObservableCollection<Ceneo> CeneoProductList
        {
            get { return _ceneoProductList; }
        }
        public ObservableCollection<Skapiec> SkapiecProductList
        {
            get { return _skapiecProductList; }
        }
        private string _AddresTextBoxText = string.Empty;
        public string AddresTextBoxText
        {
            get { return this._AddresTextBoxText; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._AddresTextBoxText, value))
                {
                    this._AddresTextBoxText = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string _ResultTextBox = "Testowe Wyszukiwania: \n gv-n960ixoc \n xbox one 500gb kinect";
        public string ResultTextBox
        {
            get { return this._ResultTextBox; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._ResultTextBox, value))
                {
                    this._ResultTextBox = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private Ceneo _selectedCeneoLista;
        public Ceneo SelectedCeneoLista
        {
            get { return _selectedCeneoLista; }
            set
            {
                if (_selectedCeneoLista == value) return;
                _selectedCeneoLista = value;
                this.RaisePropertyChanged();
                //System.Diagnostics.Debug.WriteLine("Ceneo Lista Selected:" + SelectedCeneoLista.CeneoNazwa + " URL: " + SelectedCeneoLista.CeneoUrl);
                if (_selectedSkapiecLista != null)
                {
                    ExtractButtonEnabled = true;
                    ETLButtonEnabled = true;
                }
            }
        }
        private Skapiec _selectedSkapiecLista;
        public Skapiec SelectedSkapiecLista
        {
            get { return _selectedSkapiecLista; }
            set
            {
                if (_selectedSkapiecLista == value) return;
                _selectedSkapiecLista = value;
                this.RaisePropertyChanged();
                //System.Diagnostics.Debug.WriteLine("Skapiec Lista Selected:" + SelectedSkapiecLista.SkapiecNazwa +" URL: " + SelectedSkapiecLista.SkapiecUrl);
                if (_selectedCeneoLista != null)
                {
                    ExtractButtonEnabled = true;
                    ETLButtonEnabled = true;
                }
            }
        }
        private bool _searchButtonEnabled = true;
        public bool SearchButtonEnabled
        {
            get { return _searchButtonEnabled; }
            set
            {
                if (_searchButtonEnabled == value) return;
                _searchButtonEnabled = value;
                this.RaisePropertyChanged();

            }
        }
        private bool _extractButtonEnabled = false;
        public bool ExtractButtonEnabled
        {
            get { return _extractButtonEnabled; }
            set
            {
                if (_extractButtonEnabled == value) return;
                _extractButtonEnabled = value;
                this.RaisePropertyChanged();

            }
        }
        private bool _transformButtonEnabled = false;
        public bool TransformButtonEnabled
        {
            get { return _transformButtonEnabled; }
            set
            {
                if (_transformButtonEnabled == value) return;
                _transformButtonEnabled = value;
                this.RaisePropertyChanged();

            }
        }
        private bool _loadButtonEnabled = false;
        public bool LoadButtonEnabled
        {
            get { return _loadButtonEnabled; }
            set
            {
                if (_loadButtonEnabled == value) return;
                _loadButtonEnabled = value;
                this.RaisePropertyChanged();

            }
        }
        private bool _etlButtonEnabled = false;
        public bool ETLButtonEnabled
        {
            get { return _etlButtonEnabled; }
            set
            {
                if (_etlButtonEnabled == value) return;
                _etlButtonEnabled = value;
                this.RaisePropertyChanged();

            }
        }
        private bool _progressActive = false;
        public bool ProgressActive
        {
            get { return _progressActive; }
            set
            {
                if (_progressActive == value) return;
                _progressActive = value;
                this.RaisePropertyChanged();

            }
        }

    }
}
