using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Utils;

namespace DB
{
    public class DBConteiner
    {
        private static List<Human> users;
        public List<Human> Users() { return users; }

        private FileInfo info;
        public FileInfo Info() { return info; }

        /* Временный метод пока нет бд */
        public void LoadDB(string filename)
        {
            users = new List<Human>();
            foreach (JToken user_data in FilesIO.LoadFileJson(filename))
            {
                users.Add(new Human(user_data));
            }
            info = new FileInfo(filename);
        }
    }
}
