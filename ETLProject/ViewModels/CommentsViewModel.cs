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

        public static CommentsViewModel GetForDeviceId(long deviceId)
        {
            lock (typeof(CommentsViewModel))
            {
                if (instances.ContainsKey(deviceId) != true)
                    instances[deviceId] = new CommentsViewModel(deviceId);
            }

            return instances[deviceId];
        }

        public static CommentsViewModel GetDefault()
        {
            lock (typeof(CommentsViewModel))
            {
                if (defaultInstance == null)
                    defaultInstance = new CommentsViewModel();
            }

            return defaultInstance;
        }

        public long DeviceId { get; private set; }

        protected override string GetSelectAllSql()
        {
            //this.id = id;
            //this.deviceId = deviceId;
            //this.zalety = zalety;
            //this.wady = wady;
            //this.podsumowanieOpinii = podsumowanieOpinii;
            //this.gwiazdki = gwiazdki;
            //this.autor = autor;
            //this.data = data;
            //this.polecam = polecam;
            //this.przydatna = przydatna;
            //this.pochodzenie = pochodzenie;
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

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            if (DeviceId < 0)
                return;

            statement.Bind(1, DeviceId);
        }

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

        protected override string GetSelectItemSql()
        {
            return @"SELECT Id, DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki, Autor, Data, Polecam, Przydatnosc, Pochodzenie
                           FROM Comment
                            WHERE Id = ?";
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }

        protected override string GetInsertItemSql()
        {
            return @"INSERT INTO Comment (DeviceId, Zalety, Wady, TekstOpinii, Gwiazdki,
                    Autor, Data, Polecam, Przydatnosc, Pochodzenie) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        }

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

        protected override string GetUpdateItemSql()
        {
            return @"UPDATE Comment SET DeviceId = ?, Zalety = ?, Wady = ?, TekstOpinii = ?,
                        Gwiazdki = ?, Autor = ?, Data = ?, Polecam = ?, Przydatnosc = ?, Pochodzenie = ? 
                        WHERE Id = ?";
        }

        protected override void FillUpdateStatement(ISQLiteStatement statement, long key, Comment item)
        {
            FillInsertStatement(statement, item);
            statement.Bind(11, key);
        }

        protected override string GetDeleteItemSql()
        {
            return "DELETE FROM Comment WHERE Id = ?";
        }
        protected override void FillDeleteItemStatement(ISQLiteStatement statement, long key)
        {
            statement.Bind(1, key);
        }
    }
}
