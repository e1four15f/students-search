using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class DBCreator
    {
        // TODO Генерировать уникальный идентификатор 
        private const string postfix = "_24_10_2018";
        private static Stopwatch timer = new Stopwatch();

        public List<Human> Create()
        {
            Stopwatch full_timer = new Stopwatch();
            full_timer.Start();
            JArray local_users_data = LocalGroupsSearch();

            List<string> public_groups_members_ids = PublicGroupsSearch();
            List<string> search_users_ids = DefaultUsersSearch();

            Console.WriteLine("UsersGet");
            timer.Restart();
            /* 35343 | 16.77 s */ JArray public_users_data = VkApiMulti.UsersGet(public_groups_members_ids.Union(search_users_ids).ToList(), VkApiUtils.fields);
            JArray users_data = new JArray(local_users_data.Union(public_users_data));
            timer.Stop();
            SaveFileJson("users_data", users_data);
            Console.WriteLine("Найдено " + users_data.Count() + " пользователей из миэта " + timer.Elapsed);

            List<Human> users = new List<Human>();
            foreach (JToken user_data in users_data)
            {
                users.Add(new Human(user_data));
            }
            full_timer.Stop();
            Console.WriteLine("Создание БД завершено! " + full_timer.Elapsed);
            return users;
        }

        private JArray LocalGroupsSearch()
        {
            List<string> words = new List<string>() {
                "миэт", "miet", "бмс", "bms", "bmc", 
                "мпитк", "мп", "mp", "mpitk", "вт", "vt",
                "экт", "ekt", "ect", "иняз", "inyas", "inyaz",
                "прит", "prit", "итс", "its", "мэ", "me",
                "инэуп", "ineup", "иэмс", "iems", "оиг", "oig",
                "вм", "vm", "иб", "ib", "мпв", "mpv", "мртус", "mrtus",
                "игд", "ipovs", "мфэ", "mfe", "оф", "of",
                "китис", "kitis", "п", "пэ", "саук", "sauk", "sayk",
                "кфн", "kfn", "эим", "eim", "эу", "eu",
                "миуп", "miup", "эув", "euv", "ius",
                "пкимс", "pkims", "сск", "ssk", "cck",
                "ткс", "tks", "фсип", "fcip", "иус",
                "ато", "ato", "итуутс", "ituuts"
            };
            Console.WriteLine("GroupsSearch");
            timer.Restart();
            /* 18947 | 30.22 s */ List<int> local_not_checked_groups_ids = VkApiMulti.GroupsSearch(words);
            timer.Stop();
            SaveFile("Local_not_checked_groups_ids", local_not_checked_groups_ids);
            Console.WriteLine("Найдено " + local_not_checked_groups_ids.Count() + " локальных групп " + timer.Elapsed);

            Console.WriteLine("LocalMietGroupCheck");
            timer.Restart();
            /* 10622 | 7 min 50.61 s */ JArray local_users_data = VkApiMulti.LocalMietGroupCheck(local_not_checked_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            SaveFileJson("local_users_data", local_users_data);
            Console.WriteLine("Найдено " + local_users_data.Count() + " пользователей в локальных миэтовских группах " + timer.Elapsed);

            return local_users_data;
        }

        private List<string> PublicGroupsSearch()
        {
            Console.WriteLine("GroupsSearch");
            timer.Restart();
            /* 335 | 5.49 s */ List<int> public_groups_ids = VkApiMulti.GroupsSearch(new List<string>() { "miet", "миэт" });
            timer.Stop();
            SaveFile("public_groups_ids", public_groups_ids);
            Console.WriteLine("Найдено " + public_groups_ids.Count() + " публичных миэтовских групп " + timer.Elapsed);

            Console.WriteLine("GroupsGetMembers");
            timer.Restart();
            /* 23748 | 3.26 s */ List<string> public_groups_members_ids = VkApiMulti.GroupsGetMembers(public_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            SaveFile("public_groups_members_ids", public_groups_members_ids);
            Console.WriteLine("Найдено " + public_groups_members_ids.Count() + " пользователей в публичных миэтовских группах " + timer.Elapsed);

            return public_groups_members_ids;
        }

        private List<string> DefaultUsersSearch()
        {
            Console.WriteLine("UsersSearch");
            timer.Restart();
            /* 2061 | 47.44 s */ List<string> search_users_ids = VkApiMulti.UsersSearch();
            timer.Stop();
            SaveFile("search_users_ids", search_users_ids);
            Console.WriteLine("Найдено " + search_users_ids.Count() + " пользователей из миэта по поиску " + timer.Elapsed);

            return search_users_ids;
        }
        
        // TODO Разобраться куда запихнуть сохранение/загрузку файлов
        /* IO */
        private static void SaveFile(string filename, List<int> list)
        {
            File.WriteAllText("data/" + filename + postfix, String.Join(",", list.ToArray()));
            Console.WriteLine(filename + " was saved!");
        }

        private static void SaveFile(string filename, List<string> list)
        {
            File.WriteAllText("data/" + filename + postfix, String.Join(",", list.ToArray()));
            Console.WriteLine(filename + " was saved!");
        }

        private static void SaveFileJson(string filename, JArray data)
        {
            using (StreamWriter file = File.CreateText("data/" + filename + postfix + ".json"))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    data.WriteTo(writer);
                }
            }
            Console.WriteLine(filename + " was saved!");
        }
        /*
        private static List<int> LoadFile(string filename)
        {
            string file = File.ReadAllText("data/" + filename + postfix);
            List<int> list = new List<int>();
            foreach (string value in file.Split(','))
            {
                list.Add(int.Parse(value));
            }
            Console.WriteLine(filename + " was loaded!");
            return list;
        }
        */
        private static List<string> LoadFile(string filename)
        {
            string file = File.ReadAllText("data/" + filename + postfix);
            List<string> list = new List<string>();
            foreach (string value in file.Split(','))
            {
                list.Add(value);
            }
            Console.WriteLine(filename + " was loaded!");
            return list;
        }
        
        private static JArray LoadFileJson(string filename)
        {
            JArray data = new JArray();
            using (StreamReader file = File.OpenText("data/" + filename + postfix + ".json"))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    data = JArray.Load(reader);
                }
            }
            Console.WriteLine(filename + " was loaded!");
            return data;
        }
    }
}
