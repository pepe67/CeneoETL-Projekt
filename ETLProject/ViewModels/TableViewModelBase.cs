using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.ViewModels
{/// <summary>
/// Bazowy View Model dla widoków danych z bazy SQLite.
/// Zawiera metody abstrakcyjne.
/// </summary>
/// <typeparam name="TItemType"></typeparam>
/// <typeparam name="TKeyType"></typeparam>
    public abstract class TableViewModelBase<TItemType, TKeyType>
    {/// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
        protected abstract string GetSelectAllSql();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        protected abstract void FillSelectAllStatement(ISQLiteStatement statement);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        protected abstract TItemType CreateItem(ISQLiteStatement statement);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetSelectItemSql();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected abstract void FillSelectItemStatement(ISQLiteStatement statement, TKeyType key);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetDeleteItemSql();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        protected abstract void FillDeleteItemStatement(ISQLiteStatement statement, TKeyType key);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetInsertItemSql();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="item"></param>
        protected abstract void FillInsertStatement(ISQLiteStatement statement, TItemType item);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract string GetUpdateItemSql();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        protected abstract void FillUpdateStatement(ISQLiteStatement statement, TKeyType key, TItemType item);
        /// <summary>
        /// 
        /// </summary>
        protected DateTime lastModifiedTime = DateTime.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime Timestamp
        {
            get { return lastModifiedTime; }
            protected set { lastModifiedTime = value; }
        }

        private ISQLiteConnection sqlConnection
        {
            get
            {
#if SILVERLIGHT
            return MSOpenTechSQLiteDemo.App.conn;
#else
                return ETLProject.App.conn;
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<TItemType> GetAllItems()
        {
            var items = new ObservableCollection<TItemType>();
            using (var statement = sqlConnection.Prepare(GetSelectAllSql()))
            {
                FillSelectAllStatement(statement);
                while (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    items.Add(item);
                }
            }
            Timestamp = DateTime.Now;

            return items;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TItemType GetItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetSelectItemSql()))
            {
                FillSelectItemStatement(statement, key);

                if (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    Timestamp = DateTime.Now;
                    return item;
                }
            }

            throw new ArgumentOutOfRangeException("key not found");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void InsertItem(TItemType item)
        {
            using (var statement = sqlConnection.Prepare(GetInsertItemSql()))
            {
                FillInsertStatement(statement, item);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        public void UpdateItem(TKeyType key, TItemType item)
        {
            using (var statement = sqlConnection.Prepare(GetUpdateItemSql()))
            {
                FillUpdateStatement(statement, key, item);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void DeleteItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetDeleteItemSql()))
            {
                FillDeleteItemStatement(statement, key);
                statement.Step();
            }
            Timestamp = DateTime.Now;
        }
    }
}
