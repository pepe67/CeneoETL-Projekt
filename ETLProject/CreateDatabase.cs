using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject
{
    class CreateDatabase
    {

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

            public async static Task ResetDataAsync(SQLiteConnection db)
            {
                // Empty the Customer and Project tables 
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

                //List<Task> tasks = new List<Task>();

                ////// Add seed Devices and Comments
                //var cust1Task = InsertDevice(db, "xbox One", "Microsoft", "Inne");
                //tasks.Add(cust1Task.ContinueWith((id) => InsertComment(db, id.Result, "zalety", "Wady", "autor", "opinia", "gwiazdki", "data", "Polecam", "1z2", "Opieo")));
                //tasks.Add(cust1Task.ContinueWith((id) => InsertComment(db, id.Result, "zalety", "Wady", "autor", "opinia", "gwiazdki", "data", "Polecam", "1z2", "Opieo")));
            
                //await Task.WhenAll(tasks.ToArray());

           
        }

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
        //(int)deviceId, element.zalety, element.wady, element.autor, 
        //element.podsumowanieOpinii, element.gwiazdki, element.data, element.polecam, element.przydatna, element.pochodzenie
        public async static Task<long> GetCommentId(ISQLiteConnection db, int deviceId,
            string zalety, string wady,
            string autor, string tekstOpinii)
        {
            long commIdReturn = 0;
            try
            {
                await Task.Run(() =>
                { //DeviceId, Zalety, Wady, Autor, TekstOpinii,
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


