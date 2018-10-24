using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Search
{
    class Program
    {
        static void Main(string[] args)
        {
            VkApiUtils.Auth();

            string fields = "bdate,city,connections,contacts,domain,education,exports,occupation,photo_100,sex,universities";
            Console.WriteLine(VkApi.UsersGet("whyuhookmetrash", fields));
            
            //Console.WriteLine(String.Join(",", VkApi.GroupsSearch("миэт", 3).ToArray()));
            //Console.WriteLine(String.Join(",", VkApi.GroupsGetMembers("mietoverhear", 100).ToArray())); 
            //Console.WriteLine(String.Join(",", VkApi.GroupsGetById(new List<int>() { 123, 312, 412521, 521 }).ToArray()));
            //Console.WriteLine(String.Join(",", VkApi.UsersSearch("Дима", 10).ToArray()));

            //Console.WriteLine(VkApiAdvanced.HiddenMietGroupCheck("mp_15a_2016"));
            //Console.WriteLine(String.Join(",", VkApiAdvanced.GetHiddenGroups("миэт").ToArray()));
            /*
            Stopwatch timer = new Stopwatch();
            List<int> user_ids = VkApi.GroupsGetMembers("mietoverhear");

            List<JToken> users_data = new List<JToken>();
            timer.Start();
            users_data = VkApiMulti.UsersGet(user_ids);
            timer.Stop();
            Console.WriteLine("With parallel: " + timer.Elapsed);
            */

            //TimedPublicGroupsSearch();
            //TimedCreateDB();

            //CreateDB();

            Console.WriteLine("Done");
            Console.Read();
        }

        // TODO Генерировать уникальный идентификатор 
        static string postfix = "_23_10_2018";

        static void CreateDB()
        {
            /*
            List<int> hidden_groups_members_ids = HiddenGroupsSearch();
            List<int> public_groups_members_ids = PublicGroupsSearch();
            List<int> search_users_ids = DefaultUsersSearch();
            */
            // TODO hashset
            //List<int> users_ids = hidden_groups_members_ids.Union(public_groups_members_ids).ToList().Union(search_users_ids).ToList();
            //List<int> users_ids = HiddenGroupsSearch().Union(PublicGroupsSearch()).ToList().Union(DefaultUsersSearch()).ToList();

            List<int> users_ids = LoadFile("hidden_groups_members_ids").Union(LoadFile("public_groups_members_ids")).ToList().Union(LoadFile("search_users_ids")).ToList();
            string fields = "bdate,city,connections,contacts,domain,education,exports,occupation,photo_100,sex,universities";
            JArray all_users_data = VkApiMulti.UsersGet(users_ids, fields);

            JArray users_data = new JArray();
            foreach (JToken user_data in all_users_data)
            {
                if (user_data["deactivated"] == null)
                {
                    users_data.Add(user_data);
                }
            }
            
            Console.WriteLine("В базе данных " + users_data.Count() + " пользователей");
            SaveFileJson("users_data", users_data);
            /*
             Python                 C#
             *                      4907
             ~ 1min 50s             4min 26.21s
            */
        }

        private static List<int> HiddenGroupsSearch()
        {
            /* Скрытые группы */
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
            List<int> hidden_not_checked_groups_ids = VkApiMulti.GetHiddenGroups(words);
            Console.WriteLine("Найдено " + hidden_not_checked_groups_ids.Count() + " скрытых групп");
            /*
             Python                         C#
             Найдено 4911 скрытых групп     4907
             Wall time: 31.1 s              59.3 s
            */

            List<string> hidden_groups_ids = VkApiMulti.HiddenMietGroupCheck(hidden_not_checked_groups_ids.Select(x => x.ToString()).ToList());
            Console.WriteLine("Найдено " + hidden_groups_ids.Count() + " скрытых миэтовских групп");
            /*
             Python                                 C#
             Найдено 524 скрытых миэтовских групп   520
             Wall time: 5min 45s                    19min 34s
            */

            List<int> hidden_groups_members_ids = VkApiMulti.GroupsGetMembers(hidden_groups_ids);
            Console.WriteLine("Найдено " + hidden_groups_members_ids.Count() + " пользователей в скрытых миэтовских группах");
            /*
             Python                                                     C#
             Найдено 7238 пользователей в скрытых миэтовских группах    7244
             Wall time: 8.92 s                                          3.5 s
            */

            SaveFile("hidden_groups_members_ids", hidden_groups_members_ids);
            return hidden_groups_members_ids;
        }

        private static List<int> PublicGroupsSearch()
        {
            /* Открытые группы */
            List<int> public_groups_ids = VkApiMulti.GroupsSearch(new List<string>() { "miet", "миэт" });
            Console.WriteLine("Найдено " + public_groups_ids.Count() + " публичных миэтовских групп");
            /*
             Python                                     C#
             Найдено 334 публичных миэтовских групп     333
             Wall time: 2.72 s                          0.47 s
            */
    
            List<int> public_groups_members_ids = VkApiMulti.GroupsGetMembers(public_groups_ids.Select(x => x.ToString()).ToList());
            Console.WriteLine("Найдено " + public_groups_members_ids.Count() + " пользователей в публичных миэтовских группах");
            /*
             Python                                                         C#
             Найдено 23692 пользователей в публичных миэтовских группах     23718
             Wall time: 8.25 s                                              3.07 s
            */

            SaveFile("public_groups_members_ids", public_groups_members_ids);
            return public_groups_members_ids;
        }

        private static List<int> DefaultUsersSearch()
        {
            /* Дефолтный поиск */
            List<int> search_users_ids = VkApiMulti.UsersSearch();
            Console.WriteLine("Найдено " + search_users_ids.Count() + " пользователей из миэта");
            /*
             Python             C#
             *                  4223
             *                  1min 54.69s
            */

            SaveFile("search_users_ids", search_users_ids);
            return search_users_ids;
        }

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

        // TODO Мб добавить заголовок как в json, чтобы в дальнейшем было легче работать 
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
            //File.WriteAllText("data/" + filename + postfix, String.Join(",", list.Select(x => x.ToString())));
        }
        
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

        /* Timed */
        private static void TimedHiddenGroupsSearch()
        {
            Stopwatch timer = new Stopwatch();
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
            Console.WriteLine("GetHiddenGroups");
            timer.Start();
            List<int> hidden_not_checked_groups_ids = VkApiMulti.GetHiddenGroups(words);
            timer.Stop();
            SaveFile("hidden_not_checked_groups_ids", hidden_not_checked_groups_ids);
            Console.WriteLine("Найдено " + hidden_not_checked_groups_ids.Count() + " скрытых групп " + timer.Elapsed);

            Console.WriteLine("HiddenMietGroupCheck");
            timer.Restart();
            List<string> hidden_groups_ids = VkApiMulti.HiddenMietGroupCheck(hidden_not_checked_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            SaveFile("hidden_groups_ids", hidden_groups_ids);
            Console.WriteLine("Найдено " + hidden_groups_ids.Count() + " скрытых миэтовских групп " + timer.Elapsed);

            Console.WriteLine("GroupsGetMembers");
            timer.Restart();
            List<int> hidden_groups_members_ids = VkApiMulti.GroupsGetMembers(hidden_groups_ids);
            timer.Stop();
            SaveFile("hidden_groups_members_ids", hidden_groups_members_ids);
            Console.WriteLine("Найдено " + hidden_groups_members_ids.Count() + " пользователей в скрытых миэтовских группах " + timer.Elapsed);
        }

        private static void TimedPublicGroupsSearch()
        {
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("GroupsSearch");
            timer.Start();
            List<int> public_groups_ids = VkApiMulti.GroupsSearch(new List<string>() { "miet", "миэт" });
            timer.Stop();
            SaveFile("public_groups_ids", public_groups_ids);
            Console.WriteLine("Найдено " + public_groups_ids.Count() + " публичных миэтовских групп " + timer.Elapsed);

            Console.WriteLine("GroupsGetMembers");
            timer.Restart();
            List<int> public_groups_members_ids = VkApiMulti.GroupsGetMembers(public_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            SaveFile("public_groups_members_ids", public_groups_members_ids);
            Console.WriteLine("Найдено " + public_groups_members_ids.Count() + " пользователей в публичных миэтовских группах " + timer.Elapsed);
        }

        private static void TimedDefaultUsersSearch()
        {
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("UsersSearch");
            timer.Start();
            List<int> search_users_ids = VkApiMulti.UsersSearch();
            timer.Stop();
            SaveFile("search_users_ids", search_users_ids);
            Console.WriteLine("Найдено " + search_users_ids.Count() + " пользователей из миэта " + timer.Elapsed);
        }
        /*
        private static void TimedCreateDB()
        {
            Stopwatch timer = new Stopwatch();
            Console.WriteLine("Loading");
            timer.Start();
            List<int> users_ids = LoadFile("hidden_groups_members_ids").Union(LoadFile("public_groups_members_ids")).ToList().Union(LoadFile("search_users_ids")).ToList();
            timer.Stop();
            Console.WriteLine("Loaded " + timer.Elapsed);

            timer.Restart();
            string fields = "bdate,city,connections,contacts,domain,education,exports,occupation,photo_100,sex,universities";
            List<JToken> users_data = VkApiMulti.UsersGet(users_ids, fields);
            timer.Stop();
            SaveFile("users_data", users_data);
            Console.WriteLine("В базе данных " + users_data.Count() + " пользователей " + timer.Elapsed);
        }
        */
    }
}
