using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LiteDB;

using GUI;

namespace DB
{
    class DatabaseAPI
    {
        public static readonly string DEFAULT_DB = "data/liteDB.ldb";

        private string connStr;

        public DatabaseAPI(string connStr)
        {
            this.connStr = connStr;

            using (var db = new LiteDatabase(connStr))
            {
                if (!db.CollectionExists("users"))
                {
                    db.GetCollection("users");
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

                IEnumerable<Human> result = (queries.Count > 1) ? 
                    coll.Find(Query.And(queries.ToArray())) : 
                    coll.Find(queries[0]);        

                return result.ToList();
            }
        }
        
        // Subinfo methods
        public FileInfo getDBFileInfo()
        {
            return new FileInfo(connStr);
        }

        public long getUserCount(string collName = "users")
        {
            using (var db = new LiteDatabase(connStr))
            {
                return db.GetCollection(collName).Count();
            }
        }
    }
}
