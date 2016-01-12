using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    class Opinia
    {
        public string zalety { get; private set; }
        public string wady { get; private set; }
        public string podsumowanieOpinii { get; private set; }
        public string gwiazdki { get; private set; }
        public string autor { get; private set; }
        public string data { get; private set; }
        public string polecam { get; private set; }
        public string przydatna { get; private set; }
        public string pochodzenie { get; private set; }

        public Opinia(string zalety, string wady, string podsumowanieOpinii, string gwiazdki, string autor, string data, string polecam, string przydatna, string pochodzenie)
        {
            this.zalety = zalety;
            this.wady = wady;
            this.podsumowanieOpinii = podsumowanieOpinii;
            this.gwiazdki = gwiazdki;
            this.autor = autor;
            this.data = data;
            this.polecam = polecam;
            this.przydatna = przydatna;
            this.pochodzenie = pochodzenie;
        }

        override public string ToString()
        {
            return "Opinia pochodzi z serwsu: " + pochodzenie + "\nAutor: " + autor + " \nPolecany: " + polecam + "\nOcena: " + gwiazdki + "\nData: " + data + "\nOpinia: " + podsumowanieOpinii + "\nZalety: " + zalety + "\nWady: " + wady + "\nUżytecznosc: " + przydatna + "\n\n";
        }
    }
}
