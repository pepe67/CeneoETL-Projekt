using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    /// <summary>
    /// Klasa produktu zapisanego w bazie danych
    /// </summary>
    public class Device : ViewModelBase
    {

        #region Properties

        private long id = -1;
        /// <summary>
        /// Id produktu
        /// </summary>
        public long Id
        {
            get { return id; }
            set { Set(ref id, value); }
        }

        private string name = string.Empty;
        /// <summary>
        /// Nazwa produktu
        /// </summary>
        public string Name
        {
            get { return name; }
            set { if (Set(ref name, value)) IsDirty = true; }
        }

        private string manufacturer = string.Empty;
        /// <summary>
        /// Producent produktu
        /// </summary>
        public string Manufacturer
        {
            get { return manufacturer; }
            set { if (Set(ref manufacturer, value)) IsDirty = true; }
        }

        private string others = string.Empty;
        /// <summary>
        /// Inne dane o produkcie
        /// </summary>
        public string Others
        {
            get { return others; }
            set { if (Set(ref others, value)) IsDirty = true; }
        }

        private bool isDirty = false;
        /// <summary>
        /// Sprawdzenie czy produkt jest nadpisany
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set { Set(ref isDirty, value); }
        }

        #endregion "Properties"

        internal Device()
        {
        }

        internal Device(long id, string name, string manufacturer, string others)
        {
            this.id = id;
            this.name = name;
            this.manufacturer = manufacturer;
            this.others = others;
            this.isDirty = false;
        }
        /// <summary>
        /// Sprawdzenie czy produkt jest nowy na liście.
        /// </summary>
        public bool IsNew { get { return Id < 0; } }
    }
    }

