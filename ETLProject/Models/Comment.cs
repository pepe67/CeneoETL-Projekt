using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    public class Comment : ViewModelBase
    {
        #region Properties
        private long id = -1;
        public long Id
        {
            get { return id; }
            private set { Set(ref id, value); }
        }
        private long deviceId = -1;
        public long DeviceId
        {
            get { return id; }
            private set { Set(ref deviceId, value); }
        }
        private string zalety = string.Empty;
        public string Zalety
        {
            get { return zalety; }
            set { Set(ref zalety, value); }
        }
        private string wady = string.Empty;
        public string Wady
        {
            get { return wady; }
            set { Set(ref wady, value); }
        }
        private string tekstOpinii = string.Empty;
        public string TekstOpinii
        {
            get { return tekstOpinii; }
            set { Set(ref tekstOpinii, value); }
        }
        private string gwiazdki = string.Empty;
        public string Gwiazdki
        {
            get { return gwiazdki; }
            set { Set(ref gwiazdki, value); }
        }
        private string autor = string.Empty;
        public string Autor
        {
           get { return autor; }
           set { Set(ref autor, value); }
        }
        private string data = string.Empty;
        public string Data
        {
            get { return data; }
            set { Set(ref data, value); }
        }
        private string polecam = string.Empty;
        public string Polecam
        {
            get { return polecam; }
            set { Set(ref polecam, value); }
        }
        private string przydatna = string.Empty;
        public string Przydatna
        {
            get { return przydatna; }
            set { Set(ref przydatna, value); }
        }
        private string pochodzenie = string.Empty;
        public string Pochodzenie
        {
            get { return pochodzenie; }
            set { Set(ref pochodzenie, value); }
        }

    #endregion "Properties"

        internal Comment(long deviceId) {
            this.deviceId = deviceId;
        }

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
