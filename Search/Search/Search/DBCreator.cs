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
    // Класс, который собирает данные с vk api
    class DBCreator
    {
        private static Stopwatch timer = new Stopwatch();

        /* Метод для старта сбора данных пользователей с vk
           Возвращает лист Human */
        public List<Human> Create()
        {
            Stopwatch full_timer = new Stopwatch();
            full_timer.Start();
            JArray local_users_data = LocalGroupsSearch(); //FilesIO.LoadFileJson("local_users_data");

            List<string> public_groups_members_ids = PublicGroupsSearch(); //FilesIO.LoadFile("public_groups_members_ids");
            List<string> search_users_ids = DefaultUsersSearch();//FilesIO.LoadFile("search_users_ids");

            Console.WriteLine("UsersGet");
            timer.Restart();
            /* 35343 | 16.77 s */ 
            JArray public_users_data = VkApiMulti.UsersGet(public_groups_members_ids.Union(search_users_ids).ToList(), VkApiUtils.fields);
            JArray users_data = new JArray(local_users_data.Union(public_users_data));
            timer.Stop();
            FilesIO.SaveFileJson("users_data", users_data);
            Console.WriteLine("Найдено " + users_data.Count() + " пользователей из миэта " + timer.Elapsed);

            List<Human> users = new List<Human>();
            foreach (JToken user_data in users_data)
            {
                if (user_data.Type != JTokenType.Null)
                {
                    users.Add(new Human(user_data));
                }
            }
            full_timer.Stop();
            Console.WriteLine("Создание БД завершено! " + full_timer.Elapsed);
            return users;
        }

        /* Метод для сбора данных пользователей с локальных групп
           Возвращает массив json */
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
            /* 18947 | 30.22 s */ 
            List<int> local_not_checked_groups_ids = VkApiMulti.GroupsSearch(words);
            timer.Stop();
            FilesIO.SaveFile("Local_not_checked_groups_ids", local_not_checked_groups_ids);
            Console.WriteLine("Найдено " + local_not_checked_groups_ids.Count() + " локальных групп " + timer.Elapsed);

            Console.WriteLine("LocalMietGroupCheck");
            timer.Restart();
            /* 10622 | 7 min 50.61 s */ 
            JArray local_users_data = VkApiMulti.LocalMietGroupCheck(local_not_checked_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            FilesIO.SaveFileJson("local_users_data", local_users_data);
            Console.WriteLine("Найдено " + local_users_data.Count() + " пользователей в локальных миэтовских группах " + timer.Elapsed);

            return local_users_data;
        }

        /* Метод для сбора id пользователей с публичных групп
           Возвращает лист String с id */
        private List<string> PublicGroupsSearch()
        {
            Console.WriteLine("GroupsSearch");
            timer.Restart();
            /* 335 | 5.49 s */ 
            List<int> public_groups_ids = VkApiMulti.GroupsSearch(new List<string>() { "miet", "миэт" });
            timer.Stop();
            FilesIO.SaveFile("public_groups_ids", public_groups_ids);
            Console.WriteLine("Найдено " + public_groups_ids.Count() + " публичных миэтовских групп " + timer.Elapsed);

            Console.WriteLine("GroupsGetMembers");
            timer.Restart();
            /* 23748 | 3.26 s */ 
            List<string> public_groups_members_ids = VkApiMulti.GroupsGetMembers(public_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            FilesIO.SaveFile("public_groups_members_ids", public_groups_members_ids);
            Console.WriteLine("Найдено " + public_groups_members_ids.Count() + " пользователей в публичных миэтовских группах " + timer.Elapsed);

            return public_groups_members_ids;
        }

        /* Метод для сбора id пользователей с обычного поиска
           Возвращает лист String с id */
        private List<string> DefaultUsersSearch()
        {
            Console.WriteLine("UsersSearch");
            timer.Restart();
            /* 2061 | 47.44 s */ 
            List<string> search_users_ids = VkApiMulti.UsersSearch();
            timer.Stop();
            FilesIO.SaveFile("search_users_ids", search_users_ids);
            Console.WriteLine("Найдено " + search_users_ids.Count() + " пользователей из миэта по поиску " + timer.Elapsed);

            return search_users_ids;
        }
    }
}
