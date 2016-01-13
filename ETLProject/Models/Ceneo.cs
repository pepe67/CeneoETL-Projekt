using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    /// <summary>
    /// Model listy wyszukiwania w serwisie Ceneo
    /// </summary>
    public class Ceneo
    {
        /// <summary>
        /// Nazwa produktu
        /// </summary>
        public String CeneoNazwa { get; set; }
        /// <summary>
        /// Url do danego produktu
        /// </summary>
        public String CeneoUrl { get; set; }

        
    }
    
}
