using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    /// <summary>
    /// Klasa komentarza zapisanego w bazie danych
    /// </summary>
    public class Comment : ViewModelBase
    {
        #region Properties
        private long id = -1;
        /// <summary>
        /// Id komentarza
        /// </summary>
        public long Id
        {
            get { return id; }
            private set { Set(ref id, value); }
        }
        private long deviceId = -1;
        /// <summary>
        /// Id Produktu
        /// </summary>
        public long DeviceId
        {
            get { return id; }
            private set { Set(ref deviceId, value); }
        }
        private string zalety = string.Empty;
        /// <summary>
        /// Zalety produktu
        /// </summary>
        public string Zalety
        {
            get { return zalety; }
            set { Set(ref zalety, value); }
        }
        private string wady = string.Empty;
        /// <summary>
        /// Wady produktu
        /// </summary>
        public string Wady
        {
            get { return wady; }
            set { Set(ref wady, value); }
        }
        private string tekstOpinii = string.Empty;
        /// <summary>
        /// Tekst opinii
        /// </summary>
        public string TekstOpinii
        {
            get { return tekstOpinii; }
            set { Set(ref tekstOpinii, value); }
        }
        private string gwiazdki = string.Empty;
        /// <summary>
        /// Gwiazdki w opinii
        /// </summary>
        public string Gwiazdki
        {
            get { return gwiazdki; }
            set { Set(ref gwiazdki, value); }
        }
        private string autor = string.Empty;
        /// <summary>
        /// Autor opinii
        /// </summary>
        public string Autor
        {
           get { return autor; }
           set { Set(ref autor, value); }
        }
        private string data = string.Empty;
        /// <summary>
        /// Data dodania opinii
        /// </summary>
        public string Data
        {
            get { return data; }
            set { Set(ref data, value); }
        }
        private string polecam = string.Empty;
        /// <summary>
        /// Czy produkt jest polecany przez Autora
        /// </summary>
        public string Polecam
        {
            get { return polecam; }
            set { Set(ref polecam, value); }
        }
        private string przydatna = string.Empty;
        /// <summary>
        /// Ocena przydatności komentarza przez użytkowników
        /// </summary>
        public string Przydatna
        {
            get { return przydatna; }
            set { Set(ref przydatna, value); }
        }
        private string pochodzenie = string.Empty;
        /// <summary>
        /// Pochodzenie komentarza (Ceneo/Skąpiec)
        /// </summary>
        public string Pochodzenie
        {
            get { return pochodzenie; }
            set { Set(ref pochodzenie, value); }
        }

    #endregion "Properties"

        internal Comment(long deviceId) {
            this.deviceId = deviceId;
        }
        /// <summary>
        /// Metoda sprawdzająca czy komentarz jest nowy.
        /// </summary>
        public bool IsNew { get { return Id < 0; } }

        internal Comment
            (
            long id, long deviceId, string zalety, string wady, 
            string tekstOpinii, string gwiazdki, string autor, 
            string data, string polecam, string przydatna, string pochodzenie
            )
        {
            this.id = id;
            this.deviceId = deviceId;
            this.zalety = zalety;
            this.wady = wady;
            this.tekstOpinii = tekstOpinii;
            this.gwiazdki = gwiazdki;
            this.autor = autor;
            this.data = data;
            this.polecam = polecam;
            this.przydatna = przydatna;
            this.pochodzenie = pochodzenie;
        }
    }
}
