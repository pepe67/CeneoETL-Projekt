using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    /// <summary>
    /// Klasa modelu widoku listy wyników wyszukiwania w serwisie Skąpiec
    /// </summary>
    public class Skapiec
    {
        /// <summary>
        /// Nazwa produktu prezentowana na liście
        /// </summary>
        public String SkapiecNazwa { get; set; }
        /// <summary>
        /// Adres url do produktu
        /// </summary>
        public String SkapiecUrl { get; set; }

    }
}
