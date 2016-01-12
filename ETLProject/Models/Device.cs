using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    public class Device : ViewModelBase
    {

        #region Properties

        private long id = -1;
        public long Id
        {
            get { return id; }
            set { Set(ref id, value); }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { if (Set(ref name, value)) IsDirty = true; }
        }

        private string manufacturer = string.Empty;
        public string Manufacturer
        {
            get { return manufacturer; }
            set { if (Set(ref manufacturer, value)) IsDirty = true; }
        }

        private string others = string.Empty;
        public string Others
        {
            get { return others; }
            set { if (Set(ref others, value)) IsDirty = true; }
        }

        private bool isDirty = false;
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

        public bool IsNew { get { return Id < 0; } }
    }
    }

