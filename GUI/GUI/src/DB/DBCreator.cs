using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;
using WebApi;
using GUI;

namespace DB
{
    // Класс, который собирает данные с vk api
    class DBCreator
    {
        private static Stopwatch timer = new Stopwatch();
        private string filename;

        /* Метод для старта сбора данных пользователей с vk
           Возвращает лист Human */
        public void Create(string filename,
            bool local_groups = false, bool public_groups = false, bool search = false, bool friends = true,
            bool communities_search = false, bool group_search = false)
        {
            this.filename = filename;
            Stopwatch full_timer = new Stopwatch();
            full_timer.Start();

            List<Human> users = new List<Human>();

            Parallel.ForEach(CreateVk(local_groups, public_groups, search, friends), user_data =>
            {
                lock (users)
                {
                    users.Add(new Human(user_data, "vk"));
                }
            });

            Parallel.ForEach(CreateOk(communities_search, group_search), user_data =>
            {
                lock (users)
                {
                    users.Add(new Human(user_data, "ok"));
                }
            });

            MainWindow.db = new DatabaseAPI();
            MainWindow.db.saveDB(filename);
            MainWindow.db.addUsers(users);
            MainWindow.db_users = MainWindow.db.getAllUsers();
            
            full_timer.Stop();

            Console.WriteLine("Создание БД завершено! " + full_timer.Elapsed);
        }

        /* Обработчик вк методов */
        private JArray CreateVk(bool local_groups = false, bool public_groups = false, bool search = false, bool friends = false)
        {
            JArray local_groups_members_data = local_groups ? VkLocalGroupsSearch() : new JArray();
            JArray public_groups_members_data = public_groups ? VkPublicGroupsSearch() : new JArray();
            JArray search_users_data = search ? VkDefaultUsersSearch() : new JArray();

            List<string> unique_ids = new List<string>();
            JArray users_data = new JArray();

            timer.Start();
            /* 00:00:04.7994276 */
            foreach (JArray data in new List<JArray>() { local_groups_members_data, public_groups_members_data, search_users_data })
            {
                Parallel.For(0, data.Count, i =>
                {
                    string id = data[i]["id"].ToString();
                    if (!unique_ids.Contains(id))
                    {
                        unique_ids.Add(id);
                        lock (users_data)
                        {
                            users_data.Add(data[i]);
                        }
                    }
                });
            }

            timer.Stop();
            Console.WriteLine("Всего получено информация о " + users_data.Count() + " пользователях из миэта " + timer.Elapsed);

            if (friends)
            {
                users_data = VkUsersFriendsSearch(unique_ids);
            }

            return users_data;
        }

        /* Метод для сбора данных пользователей с локальных групп
           Возвращает массив json */
        private JArray VkLocalGroupsSearch()
        {
            List<string> words = new List<string>() 
            {
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
            /* 18954 | 1 min 0.40 s */
            List<int> local_not_checked_groups_ids = VkApiMulti.GroupsSearch(words); 
            timer.Stop();
            Console.WriteLine("Найдено " + local_not_checked_groups_ids.Count() + " локальных групп " + timer.Elapsed);

            Console.WriteLine("LocalMietGroupCheck");
            timer.Restart();
            /* 7517 | 8 min 59.31 s */ 
            JArray local_groups_members_data = VkApiMulti.LocalMietGroupCheck(local_not_checked_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            Console.WriteLine("Получена информация о " + local_groups_members_data.Count() + " пользователях в локальных миэтовских группах " + timer.Elapsed);

            return local_groups_members_data;
        }

        /* Метод для сбора данных пользователей с публичных групп
           Возвращает массив json */
        private JArray VkPublicGroupsSearch()
        {
            Console.WriteLine("GroupsSearch");
            timer.Restart();
            /* 335 | 16.42 s */ 
            List<int> public_groups_ids = VkApiMulti.GroupsSearch(new List<string>() { "miet", "миэт" });
            timer.Stop();
            Console.WriteLine("Найдено " + public_groups_ids.Count() + " публичных миэтовских групп " + timer.Elapsed);

            Console.WriteLine("GroupsGetMembers");
            timer.Restart();
            /* 20330 | 54.46 s */
            JArray public_groups_members_data = VkApiMulti.GroupsGetMembers(public_groups_ids.Select(x => x.ToString()).ToList());
            timer.Stop();
            Console.WriteLine("Получена информация о " + public_groups_members_data.Count() + " пользователях в публичных миэтовских группах " + timer.Elapsed);

            return public_groups_members_data;
        }

        /* Метод для сбора данных пользователей с обычного поиска
           Возвращает массив json */
        private JArray VkDefaultUsersSearch()
        {
            Console.WriteLine("UsersSearch");
            timer.Restart();
            /* 4129 | 1 min 37.36 s */ 
            List<string> search_users_ids = VkApiMulti.UsersSearch();
            timer.Stop();
            Console.WriteLine("Найдено " + search_users_ids.Count() + " пользователей из миэта по поиску " + timer.Elapsed);

            Console.WriteLine("UsersGet");
            timer.Restart();
            /* 4129 | 3.87 s */
            JArray search_users_data = VkApiMulti.UsersGet(search_users_ids, VkApiUtils.fields, "search");
            timer.Stop();
            Console.WriteLine("Получена информация о " + search_users_data.Count() + " пользователях из миэта по поиску " + timer.Elapsed);

            return search_users_data;
        }

        /* Метод для сбора данных друзей по списку пользователей
           Возвращает массив json */
        // TODO Долго работает, много мусора
        public JArray VkUsersFriendsSearch(List<string> users)
        {
            Console.WriteLine("FriendsGet");
            timer.Restart();
            /* ???? | ???? */
            HashSet<string> unique_ids = new HashSet<string>(users);
            foreach(string user_id in users)
            {
                HashSet<string> friends_ids = new HashSet<string>(VkApi.FriendsGet(user_id));
//                int trusted_friends = 0;
                /*
                foreach (string check_friend_id in friends_ids)
                {
                    if (users.Contains(check_friend_id))
                    {
                        trusted_friends++;
                        
                    }
                }*/
                if (users.Count(friends_ids.Contains) > friends_ids.Count() * (50.0 / 100.0))
                {
                    foreach (string friend_id in friends_ids)
                    {
                        unique_ids.Add(friend_id);
                    }

                }
            }
            timer.Stop();
            Console.WriteLine("Найдено " + unique_ids.Count() + " друзей " + timer.Elapsed);

            Console.WriteLine("UsersGet");
            timer.Restart();
            /* ???? | ???? */
            JArray users_friends_data = VkApiMulti.UsersGet(unique_ids.ToList(), VkApiUtils.fields, "search");
            timer.Stop();
            Console.WriteLine("Получена информация о " + users_friends_data.Count() + " пользователях и друзьях " + timer.Elapsed);

            return users_friends_data;
        }

        /* Обработчик ок методов */
        public JArray CreateOk(bool communities_search = false, bool group_search = false)
        {
            JArray communities_search_data = communities_search ? OkCommunitiesSearch() : new JArray();
            JArray group_search_data = group_search ? OkGroupSearch() : new JArray();

            List<string> unique_ids = new List<string>();
            JArray users_data = new JArray();

            timer.Start();
            /* 00:00:04.7994276 */
            foreach (JArray data in new List<JArray>() { communities_search_data, group_search_data })
            {
                Parallel.For(0, data.Count, i =>
                {
                    string id = data[i]["uid"].ToString();
                    if (!unique_ids.Contains(id))
                    {
                        unique_ids.Add(id);
                        lock (users_data)
                        {
                            users_data.Add(data[i]);
                        }
                    }
                });
            }
            timer.Stop();
            Console.WriteLine("Всего получено информация о " + users_data.Count() + " пользователях из миэта " + timer.Elapsed);

            return users_data;
        }

        /* Метод для сбора данных пользователей с комьюнити
           Возвращает массив json */
        private JArray OkCommunitiesSearch()
        {
            Console.WriteLine("CommunitiesGetMembers");
            timer.Restart();
            /* 12330 | 17.51 s */
            JArray communities_search_data = OkApiMulti.CommunitiesGetMembers(OkApiUtils.communities, OkApiUtils.fields);
            timer.Stop();
            Console.WriteLine("Получена информация о " + communities_search_data.Count() + " пользователей из миэтовских комьюнити " + timer.Elapsed);

            //FilesIO.SaveFileJson("ok-communities.json", data);
            return communities_search_data;
        }

        /* Метод для сбора данных пользователей с групп
           Возвращает массив json */
        private JArray OkGroupSearch()
        {
            Console.WriteLine("GroupGetMembers");
            timer.Restart();
            /* 1760 | 00.31 s */
            List<string> group_search_uids = OkApiMulti.GroupGetMembers(OkApiUtils.open_groups);
            timer.Stop();
            Console.WriteLine("Найдено " + group_search_uids.Count() + " пользователей из миэтовских групп " + timer.Elapsed);

            Console.WriteLine("UsersGetInfo");
            timer.Restart();
            /* 1760 | 00.10 s */
            JArray group_search_data = OkApiMulti.UsersGetInfo(group_search_uids, OkApiUtils.fields);
            timer.Stop();
            Console.WriteLine("Получена информация о " + group_search_uids.Count() + " пользователях из миэтовских групп " + timer.Elapsed);

            //FilesIO.SaveFileJson("ok-groups.json", data);
            return group_search_data;
        }
    }
}
