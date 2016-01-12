using ETLProject.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using System.Diagnostics;

namespace ETLProject.ViewModels
{
    public class DevicesViewModel : TableViewModelBase<Device, long>
    {
       

       public DevicesViewModel(){ }

        static DevicesViewModel instance;

        public static DevicesViewModel GetDefault()
        {
            lock (typeof(DevicesViewModel))
            {
                if (instance == null)
                    instance = new DevicesViewModel();
            }
            return instance;
        }

        protected override string GetSelectAllSql()
        {
            return "SELECT Id, Name, Manufacturer, Others FROM Device";
        }

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            // nothing to do
        }

        protected override Device CreateItem(ISQLiteStatement statement)
        {
            var d = new Device(
                (long)statement[0],
                (string)statement[1],
                (string)statement[2],
                (string)statement[3]);
            Debug.WriteLine("Selected Device name: " + d.Name);

            return d;
        }

        protected override string GetSelectItemSql()
        {
            return "SELECT Id, Name, Manufacturer, Others FROM Device WHERE Id = ?";
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

        protected override string GetInsertItemSql()
        {
            return "INSERT INTO Device (Name, Manufacturer, Others ) VALUES (@name, @manufacturer, @others)";
        }

        protected override void FillInsertStatement(ISQLiteStatement statement, Device item)
        {
            statement.Bind("@name", item.Name);
            statement.Bind("@manufacturer", item.Manufacturer);
            statement.Bind("@others", item.Others);
        }

        protected override string GetUpdateItemSql()
        {
            return "UPDATE Device SET Name= ?, Manufacturer = ?, Others = ? WHERE Id = ?";
        }

        protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Device item)
        {
            statement.Bind(1, item.Name);
            statement.Bind(2, item.Manufacturer);
            statement.Bind(3, item.Others);
            statement.Bind(4, key);
        }

        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM Device WHERE Id = ?";
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

    }
}
