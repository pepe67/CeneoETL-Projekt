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
    /// <summary>
    /// ViewModel dla Produktów
    /// </summary>
    public class DevicesViewModel : TableViewModelBase<Device, long>
    {
       
        /// <summary>
        /// Konstruktor ViewModelu
        /// </summary>
       public DevicesViewModel(){ }

        static DevicesViewModel instance;

        /// <summary>
        /// Pobranie instancji ViewModelu
        /// </summary>
        /// <returns></returns>
        public static DevicesViewModel GetDefault()
        {
            lock (typeof(DevicesViewModel))
            {
                if (instance == null)
                    instance = new DevicesViewModel();
            }
            return instance;
        }
        /// <summary>
        /// Pobranie wszystkich Produktów z tabeli Device
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectAllSql()
        {
            return "SELECT Id, Name, Manufacturer, Others FROM Device";
        }
        /// <summary>
        /// Wypełnienie Selecta, jednak nie ma czego wypełniać. Nadpisanie metody jest wymagane przez kompilator.
        /// (Albo wszystkie albo nic :) )
        /// </summary>
        /// <param name="statement"></param>
        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            // nothing to do
        }

        /// <summary>
        /// Dodanie nowego produktu do bazy
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pobranie produktu o danym ID
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectItemSql()
        {
            return "SELECT Id, Name, Manufacturer, Others FROM Device WHERE Id = ?";
        }

        /// <summary>
        /// Wypełnienie zapytania pobierającego Produkt o danym ID 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

        /// <summary>
        /// Zapytanie dodające nowy produkt do bazy
        /// </summary>
        /// <returns></returns>
        protected override string GetInsertItemSql()
        {
            return "INSERT INTO Device (Name, Manufacturer, Others ) VALUES (@name, @manufacturer, @others)";
        }
        /// <summary>
        /// Wypełnienie zapytania odającefo nowy produkt do bazy
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="item"></param>
        protected override void FillInsertStatement(ISQLiteStatement statement, Device item)
        {
            statement.Bind("@name", item.Name);
            statement.Bind("@manufacturer", item.Manufacturer);
            statement.Bind("@others", item.Others);
        }
        /// <summary>
        /// Zapytanie aktualizujące Produkt o danym ID
        /// </summary>
        /// <returns></returns>
        protected override string GetUpdateItemSql()
        {
            return "UPDATE Device SET Name= ?, Manufacturer = ?, Others = ? WHERE Id = ?";
        }
        /// <summary>
        /// Wypełnienie zapytania aktualizującego produkt o danym ID
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Device item)
        {
            statement.Bind(1, item.Name);
            statement.Bind(2, item.Manufacturer);
            statement.Bind(3, item.Others);
            statement.Bind(4, key);
        }
        /// <summary>
        /// Kasowanie produktu o danym ID
        /// </summary>
        /// <returns></returns>
        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM Device WHERE Id = ?";
        }
        /// <summary>
        /// Wypełnienie zapytania kasującego produkt o danym ID
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

    }
}
