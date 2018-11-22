using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Concurrent;
using System.Configuration;

using LiteDB;

using GUI;
using Utils;

namespace DB
{
    public class DatabaseAPI
    {
        public string connStr;
        public bool corrupted;

        public void saveDB(string connStr)
        {
            File.Delete(connStr);
            this.connStr = connStr;
            using (var db = new LiteDatabase(connStr))
            {
                if (!db.CollectionExists("users"))
                { 
                    db.GetCollection("users");
                }
            }
        }
       
        public void loadDB(string connStr)
        {
            this.connStr = connStr;
            using (var db = new LiteDatabase(connStr))
            {
                // TODO Если юзер пытается загрузить пустую или испорченную бд
                try
                {
                    if (db.CollectionExists("users"))
                    {

                        MainWindow.db_users = getAllUsers();

                        // Сохраним путь к бд в конфиг, чтобы в следующий раз само открылось
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings["db_destination"].Value = connStr;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");

                        corrupted = false;
                    }
                }
                catch (LiteException)
                {
                    Console.WriteLine(this.ToString() + ": Испорченная база данных");
                    MessageBox.Show("Невозможно загрузить базу данных!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    corrupted = true;
                }
            }
        }

        public bool addUser(Human human)
        {
            using (var db = new LiteDatabase(connStr))
            {
                var coll = db.GetCollection<Human>("users");
                coll.Insert(human);

                return true;
            }
        }

        public bool addUsers(List<Human> humans)
        {
            using (var db = new LiteDatabase(connStr))
            {
                var coll = db.GetCollection<Human>("users");
                foreach(Human human in humans)
                {
                    coll.Insert(human);
                }

                return true;      
            }
        }

        public List<Human> getAllUsers()
        {
            using (var db = new LiteDatabase(connStr))
            {
                var coll = db.GetCollection<Human>("users");

                return coll.FindAll().ToList();
            }
        }

        delegate bool del(Human p, bool bl);

        public List<Human> search(MakeRequest request)
        {
            if (MainWindow.db_users.Count > 0)
            { 
                using (var db = new LiteDatabase(connStr))
                {
                    var coll = db.GetCollection<Human>("users");

                    coll.EnsureIndex(p => p.first_name);
                    coll.EnsureIndex(p => p.last_name);
                    coll.EnsureIndex(p => p.sex);

                    List<Query> queries = new List<Query>();

                    // First name criteria
                    if (request.FirstName.Text.Length > 0)
                    {
                        queries.Add(Query.Contains("LOWER($.first_name)", request.FirstName.Text.ToLower()));
                    }
                    // Last name criteria
                    if (request.LastName.Text.Length > 0)
                    {
                        queries.Add(Query.Contains("LOWER($.last_name)", request.LastName.Text.ToLower()));
                    }
                    // Sex criteria
                    if (request.ManSex.IsChecked.Value && !request.FemaleSex.IsChecked.Value)
                    {
                        queries.Add(Query.EQ("sex", true));
                    } 
                    else if (!request.ManSex.IsChecked.Value && request.FemaleSex.IsChecked.Value)
                    {
                        queries.Add(Query.EQ("sex", false));
                    }
                    // Faculty name criteria
                    if (request.FacultyName.Text.Length > 0)
                    {
                        queries.Add(Query.Contains("LOWER($.universities[*].faculty_name)", request.FacultyName.Text.ToLower()));
                    }
                    // Chair name criteria
                    if (request.ChairName.Text.Length > 0)
                    {
                        queries.Add(Query.Contains("LOWER($.universities[*].chair_name)", request.ChairName.Text.ToLower()));
                    }
                    // Chair name criteria
                    if (request.ChairName.Text.Length > 0)
                    {
                        queries.Add(Query.Contains("LOWER($.universities[*].chair_name)", request.ChairName.Text.ToLower()));
                    }
                    // Graduation year criteria
                    if (request.GraduationYear.Text.Length > 0)
                    {
                        queries.Add(Query.EQ("universities[0].graduation_year", int.Parse(request.GraduationYear.Text)));
                    }

                    // Empty request
                    if (queries.Count < 1)
                    {
                        return getAllUsers().OrderByDescending(x => x.plausibility).ToList();
                    }

                    IEnumerable<Human> result = (queries.Count > 1) ? 
                        coll.Find(Query.And(queries.ToArray())) : 
                        coll.Find(queries[0]);        

                    // Sort by plausibility
                    result = result.OrderByDescending(x => x.plausibility);

                    return result.ToList();
                }
            }
            else
            {
                return new List<Human>();
            }
        }

        // Subinfo methods
        public FileInfo getDBFileInfo()
        {
            return new FileInfo(connStr);
        }
        /*
        public long getUserCount(string collName = "users")
        {
            using (var db = new LiteDatabase(connStr))
            {
                return db.GetCollection(collName).Count();
            }
        }
         * */
    }
}
