using ETLProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using System.Diagnostics;

namespace ETLProject.ViewModels
{
    /// <summary>
    /// ViewModel dla komentarzy
    /// </summary>
    public class CommentsViewModel : TableViewModelBase<Comment, long>
    {
        private CommentsViewModel(long deviceId)
        {
            DeviceId = deviceId;
        }

        private CommentsViewModel()
        {
            DeviceId = -1;
        }

        static Dictionary<long, CommentsViewModel> instances = new Dictionary<long, CommentsViewModel>();
        static CommentsViewModel defaultInstance;

        public override DateTime Timestamp
        {
            get
            {
                if (this == defaultInstance || defaultInstance == null)
                    return base.Timestamp;

                return defaultInstance.Timestamp;
            }

            protected set
            {
                if (this == defaultInstance || defaultInstance == null)
                    base.Timestamp = value;
                else
                    defaultInstance.Timestamp = value;
            }
        }
        /// <summary>
        /// Pobranie komentarza dla danego Id Produktu
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static CommentsViewModel GetForDeviceId(long deviceId)
        {
            lock (typeof(CommentsViewModel))
            {
                if (instances.ContainsKey(deviceId) != true)
                    instances[deviceId] = new CommentsViewModel(deviceId);
            }

            return instances[deviceId];
        }

        /// <summary>
        /// Domyślne pobieranie danych z tabeli Komentarzy
        /// </summary>
        /// <returns></returns>
        public static CommentsViewModel GetDefault()
        {
            lock (typeof(CommentsViewModel))
            {
                if (defaultInstance == null)
                    defaultInstance = new CommentsViewModel();
            }

            return defaultInstance;
        }
        /// <summary>
        /// Id produktu potrzebny do obsługi zapytań
        /// </summary>
        public long DeviceId { get; private set; }

        /// <summary>
        /// Pobranie wszystkich komentarzy z tabeli Comment. Lub pobranie wszystkich komentarzy dla danego ID Produktu.
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectAllSql()
        {
            
            if (DeviceId < 0)
                return @"SELECT Id, DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki, Autor, Data, Polecam, Przydatnosc, Pochodzenie
                           FROM Comment
                            ORDER BY Id";
            else
                return @"SELECT Id, DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki, Autor, Data, Polecam, Przydatnosc, Pochodzenie
                           FROM Comment
                            WHERE DeviceId = ?
                            ORDER BY Id";
        }
        /// <summary>
        /// Metoda uzupełniająca zapytanie pobierające wszystkie komentarze z bazy.
        /// Jeżeli Id Produktu jest mniejsze niż 0, to pobierane są wszystkie komentarze.
        /// </summary>
        /// <param name="statement"></param>
        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            if (DeviceId < 0)
                return;

            statement.Bind(1, DeviceId);
        }
        /// <summary>
        /// Tworzenie nowego obiektu w tabeli Comment
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        protected override Comment CreateItem(ISQLiteStatement statement)
        {
            Comment comment = new Comment(
                (long)statement[0],
                (long)statement[1],
                (string)statement[2],
                (string)statement[3],
                (string)statement[4],
                (string)statement[5],
                (string)statement[6],
                (string)statement[7],
                (string)statement[8],
                (string)statement[9],
                (string)statement[10]
             );
            Debug.WriteLine("Selected Comment author: ", comment.Autor);
            return comment;
        }
        /// <summary>
        /// Pobranie jednego komentarza z tabeli Comment
        /// </summary>
        /// <returns></returns>
        protected override string GetSelectItemSql()
        {
            return @"SELECT Id, DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki, Autor, Data, Polecam, Przydatnosc, Pochodzenie
                           FROM Comment
                            WHERE Id = ?";
        }
        /// <summary>
        /// Wypełnienie zapytania pobierającego jeden komentarz z bazy
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }
        /// <summary>
        /// Zapytanie dodające nowy komentarz do bazy
        /// </summary>
        /// <returns></returns>
        protected override string GetInsertItemSql()
        {
            return @"INSERT INTO Comment (DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki,
                    Autor, Data, Polecam, Przydatnosc, Pochodzenie) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        }
        /// <summary>
        /// Wypełnienie zapytania dodającego jeden komentarz z bazy
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="item"></param>
        protected override void FillInsertStatement(ISQLiteStatement statement, Comment item)
        {
            statement.Bind(1, item.DeviceId);
            statement.Bind(2, item.Zalety);
            statement.Bind(3, item.Wady);
            statement.Bind(4, item.TekstOpinii);
            statement.Bind(5, item.Gwiazdki);
            statement.Bind(6, item.Autor);
            statement.Bind(7, item.Data);
            statement.Bind(8, item.Polecam);
            statement.Bind(9, item.Przydatna);
            statement.Bind(10, item.Pochodzenie);

        }
        /// <summary>
        /// Zapytanie aktualizujące komentarz w bazie
        /// </summary>
        /// <returns></returns>
        protected override string GetUpdateItemSql()
        {
            return @"UPDATE Comment SET DeviceId = ?, Zalety = ?, Wady = ?, TekstOpinii = ?,
                        Gwiazdki = ?, Autor = ?, Data = ?, Polecam = ?, Przydatnosc = ?, Pochodzenie = ? 
                        WHERE Id = ?";
        }
        /// <summary>
        /// Wypełnienie zapytania aktualizującego komentarz
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Comment item)
        {
            FillInsertStatement(statement, item);
            statement.Bind(11, key);
        }
        /// <summary>
        /// Zapytanie kasujące komentarz o danym ID
        /// </summary>
        /// <returns></returns>
        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM Comment WHERE Id = ?";
        }
        /// <summary>
        /// Wypełnienie zapytania kasującego komentarz.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }
    }
}
