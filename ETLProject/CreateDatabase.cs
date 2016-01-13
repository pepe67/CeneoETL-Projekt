using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject
{
    /// <summary>
    /// Klasa CreateDatabase zajmująca się obsługą bazy danych. Wliczając w to Tworzenie odpowiednich tabeli, relacji między nimi,
    /// dodawania danych i ich pobierania.
    /// </summary>
    class CreateDatabase
    {
            /// <summary>
            /// Metoda ma na celu utworzenie Tabeli w bazie danych (jeżeli nie istnieją i stworzenie odpowiednich relacji.
            /// Ważnym elementem metody jest włączenie opcji relacyjnej bazy danych.
            /// </summary>
            /// <param name="db"></param>
            public static void LoadDatabase(SQLiteConnection db)
            {
                string sql = @"CREATE TABLE IF NOT EXISTS
                                Device (Id      INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            Name    VARCHAR( 140 ),
                                            Manufacturer    VARCHAR( 140 ),
                                            Others VARCHAR( 140 ) 
                            );";
                using (var statement = db.Prepare(sql))
                {
                    statement.Step();
                }

                sql = @"CREATE TABLE IF NOT EXISTS
                                Comment (Id          INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                         DeviceId   INTEGER,
                                         Zalety        VARCHAR( 140 ),
                                         Wady VARCHAR( 140 ),
                                         Autor VARCHAR( 140 ),
                                         TekstOpinii VARCHAR( 140 ),
                                         Gwiazdki VARCHAR( 140 ),
                                         Data VARCHAR( 140 ),
                                         Polecam VARCHAR( 140 ),
                                         Przydatnosc VARCHAR( 140 ),
                                         Pochodzenie VARCHAR( 140 ),
                                         FOREIGN KEY(DeviceId) REFERENCES Device(Id) ON DELETE CASCADE 
                            )";

            using (var statement = db.Prepare(sql))
                {
                    statement.Step();
                }

                // Turn on Foreign Key constraints
                sql = @"PRAGMA foreign_keys = ON";
                using (var statement = db.Prepare(sql))
                {
                    statement.Step();
                }
            }

        /// <summary>
        /// Metoda czyszcząca bazę danych ze wszystkich informacji.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
            public async static Task ResetDataAsync(SQLiteConnection db)
            {
                // Empty the Device and Comment tables 
                string sql = @"DELETE FROM Device";
                using (var statement = db.Prepare(sql))
                {
                    statement.Step();
                }

                sql = @"DELETE FROM Comment";
                using (var statement = db.Prepare(sql))
                {
                    statement.Step();
                }


           
        }
        /// <summary>
        /// Metoda dodająca nowy Produkt do bazy danych. Metoda jest używana w procesie ETL.
        /// Dodatkowo zwracany jest ID dodanego urządzenia.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="deviceName">Nazwa produktu pobrana w procesie ETL</param>
        /// <param name="manufacturer">Producent produktu pobrany w procesie ETL</param>
        /// <param name="others">Inne dane produktu pobrane w procesie ETL</param>
        /// <returns></returns>
            public async static Task<long> InsertDevice(ISQLiteConnection db, string deviceName, string manufacturer, string others)
            {
                try
                {
                    await Task.Run(() =>
                    {

                        using (var devicemt = db.Prepare("INSERT INTO Device (Name, Manufacturer, Others) VALUES (?, ?, ?)"))
                        {
                            devicemt.Bind(1, deviceName);
                            devicemt.Bind(2, manufacturer);
                            devicemt.Bind(3, others);
                            devicemt.Step();

                            Debug.WriteLine("INSERT completed OK for device " + deviceName);
                        }
                    }
                    );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }

                using (var idstmt = db.Prepare("SELECT last_insert_rowid()"))
                {
                    idstmt.Step();
                    {
                        Debug.WriteLine("INSERT ID for device " + deviceName + ": " + (long)idstmt[0]);
                        return (long)idstmt[0];
                    }
                    throw new Exception("Couldn't get inserted ID");
                };
            }
        /// <summary>
        /// Metoda dodająca nowy komentarz dla danego produktu. Używana w procesie ETL.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="deviceID">Id urządzenia z bazy danych</param>
        /// <param name="zalety">Zalety wymienione w komentarzu, pobrane w procesie ETL</param>
        /// <param name="wady">Wady wymienione w komentarzu, pobrane w procesie ETL</param>
        /// <param name="autor">Autor komentarza, pobrany w procesie ETL</param>
        /// <param name="tekstOpinii">Tekst Opinii pobrany w procesie ETL</param>
        /// <param name="gwiazdki">Ocena wystawiona przez autora opinii. (Zaokrąglana do pełnych liczb w dół)</param>
        /// <param name="data">Data dodania opinii na stonę</param>
        /// <param name="polecam">Czy produkt jest polecany przez kupującego. Pole dotyczy jedynie strony Ceneo</param>
        /// <param name="przydatnosc">Ocena przydatności komentarza.</param>
        /// <param name="pochodzenie">Pochodzenie komentarza. Dzięki temu łatwo można odróżnić komentarze pobrane z Ceneo, od komentarzy pobranych ze Skąpca.</param>
        /// <returns></returns>
        public static Task InsertComment(ISQLiteConnection db, long deviceID, string zalety, string wady, string autor, string tekstOpinii, string gwiazdki, string data, string polecam, string przydatnosc, string pochodzenie)
            {
                return Task.Run(() =>
                {

                    using (var commentmt = db.Prepare("INSERT INTO Comment (DeviceId, Zalety, Wady, Autor, TekstOpinii, Gwiazdki, Data, Polecam, Przydatnosc, Pochodzenie) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"))
                    {

                        // Reset the prepared statement so we can reuse it.
                        commentmt.ClearBindings();
                        commentmt.Reset();

                        commentmt.Bind(1, deviceID);
                        commentmt.Bind(2, zalety);
                        commentmt.Bind(3, wady);
                        commentmt.Bind(4, autor);
                        commentmt.Bind(5, tekstOpinii);
                        commentmt.Bind(6, gwiazdki);
                        commentmt.Bind(7, data);
                        commentmt.Bind(8, polecam);
                        commentmt.Bind(9, przydatnosc);
                        commentmt.Bind(10, pochodzenie);

                        commentmt.Step();
                        Debug.WriteLine("INSERT Project completed OK for comment " + deviceID);
                    }
                }
                );
        }
        /// <summary>
        /// Metoda ma na celu zwrócenie ID dowolnego produktu - zależnie od potrzeb.
        /// Używana w czasie dodawania komentarzy. Dzięki niej produkty nie dublują się.
        /// </summary>
        /// <param name = "db" ></param>
        ///<param name = "deviceName" > Nazwa produktu</param>
        /// <param name="manufacturer">Producent produktu</param>
        /// <param name="others">Inne dane produktu</param>
        /// <returns></returns>
        public async static Task<long> GetDeviceId(ISQLiteConnection db, string deviceName, string manufacturer, string others)
        {
            long devIdReturn = 0;
            try
            {
                await Task.Run(() =>
                {
                    using (var deviceId = db.Prepare("SELECT Id FROM Device WHERE Name = ? AND Manufacturer = ? AND Others = ?"))
                    {
                        deviceId.Bind(1, deviceName);
                        deviceId.Bind(2, manufacturer);
                        deviceId.Bind(3, others);

                        deviceId.Step();
                        Debug.WriteLine(" GET ID For " + deviceName);
                        try
                        {
                            return devIdReturn = (long)deviceId[0];      
                        }
                        catch
                        {
                           return devIdReturn = 0;
                        }
                    }
                    
                });
                Debug.WriteLine("ID is: " + devIdReturn);
                return devIdReturn;
            }
            catch
            {
                return devIdReturn;
            }
        }

        /// <summary>
        /// Metoda pobierająca ID dowolnego komentarza. Jest używana w czasie dodawania nowych komentarzy.
        /// Dzięki niej komentarze nie są dublowane. W celu przyśpieszenia zapytania, nie są sprawdzane wszystkie pola.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="deviceId">Id urządzenia</param>
        /// <param name="zalety">Zalety wymienione w komentarzu</param>
        /// <param name="wady">Wady wymienione w komentarzu</param>
        /// <param name="autor">Autor komentarza</param>
        /// <param name="tekstOpinii">Tekst Opinii</param>
        /// <returns></returns>
        public async static Task<long> GetCommentId(ISQLiteConnection db, int deviceId,
            string zalety, string wady,
            string autor, string tekstOpinii)
        {
            long commIdReturn = 0;
            try
            {
                await Task.Run(() =>
                { 
                    using (var commId = db.Prepare("SELECT Id FROM Comment WHERE DeviceId = ? AND Zalety = ? AND Wady = ? AND Autor = ? AND TekstOpinii = ?"))
                    {
                        commId.Bind(1, deviceId);
                        commId.Bind(2, zalety);
                        commId.Bind(3, wady);
                        commId.Bind(4, autor);
                        commId.Bind(5, tekstOpinii);

                        commId.Step();
                        Debug.WriteLine(" GET ID For " + autor);
                        try
                        {
                            return commIdReturn = (long)commId[0];
                        }
                        catch
                        {
                            return commIdReturn = 0;
                        }
                    }

                });
                Debug.WriteLine("Comment ID is: " + commIdReturn);
                return commIdReturn;
            }
            catch
            {
                return commIdReturn;
            }
        }

    }

    }


