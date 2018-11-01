using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi
{
    // Класс с распаралеленными методами
    abstract class VkApiMulti
    {
        /* Методы из vk api */

        /* Метод позволяющий получить подробную информацию о пользователях в листе int по id
           Возвращает массив json с информацией всех пользователей */
        internal static JArray UsersGet(List<string> user_ids, string fields = "", string arrived_from = "") 
        {
            JArray users_data = new JArray();
            Parallel.For (0, user_ids.Count / 350 + 1, i =>
            {
                int max_value = i * 350 + 350 > user_ids.Count ? user_ids.Count - i * 350 : 350;
                JArray ranged_users_data = VkApi.UsersGet(user_ids.GetRange(i * 350, max_value), VkApiUtils.fields, arrived_from);
                for (int j = 0; j < ranged_users_data.Count; j++)
                {
                    lock (users_data) users_data.Add(ranged_users_data[j]);
                }
            });
            
            return users_data;
        }
        
        /* Метод позволяющий выполнить поиск групп по текстовым строкам из листа string
           Возвращает лист int с id групп */
        internal static List<int> GroupsSearch(List<string> texts, short count = 1000) 
        {
            List<List<string>> splited_texts = new List<List<string>>();
            for (int i = 0; i < texts.Count; i += 4)
            {
                splited_texts.Add(texts.GetRange(i, Math.Min(4, texts.Count - i)));
            } 

            HashSet<int> group_ids = new HashSet<int>();
            Parallel.ForEach (splited_texts, part_texts =>
            {
                foreach (string text in part_texts)
                {
                    Thread.Sleep(VkApiUtils.sleep_time);
                    foreach (int group_id in VkApi.GroupsSearch(text, count))
                    {
                        lock (group_ids) group_ids.Add(group_id);
                    }
                }
            });

            return group_ids.ToList<int>();
        }

        /* Метод позволяющий получить всех пользователей из групп в листе string
           Возвращает лист int с id всех пользователей */
        internal static JArray GroupsGetMembers(List<string> group_ids, int max_count = 0) 
        {
            List<string> unique_ids = new List<string>();
            
            JArray users_data = new JArray();
            Parallel.ForEach (group_ids, group_id =>
            {
                JArray group_users_data = VkApiAdvanced.GroupsGetMembers(group_id, max_count);
                for (int i = 0; i < group_users_data.Count; i++)
                {
                    string id = group_users_data[i]["id"].ToString();
                    if (!unique_ids.Contains(id))
                    {
                        unique_ids.Add(id);
                        lock (users_data) users_data.Add(group_users_data[i]);
                    }
                }
            });

            return users_data;
        }
        
        /* Метод позволяющий произвести поиск людей по всем параметрам
           Возвращает лист int с id найденных пользователей */
        // TODO Задавать age=range(0, 120), sex=range(1, 3)
        internal static List<string> UsersSearch(string q = "", short count = 1000, short age_min = 0, short age_max = 120) //, List<short> age = 0, byte sex = 0)
        {
            HashSet<string> user_ids = new HashSet<string>();
            Parallel.For(1, 3, sex =>
            {
                Thread.Sleep(VkApiUtils.sleep_time);
                for (int age = age_min; age < age_max; age++)
                {
                    foreach (string user_id in VkApi.UsersSearch(q, count, (short) age, (byte) sex))
                    {
                        lock (user_ids) user_ids.Add(user_id);
                    }
                }
            });
            
            return user_ids.ToList<string>();
        }

        /* Методы из vk api advanced */

        /* Метод проверяющий скрытые группы из листа string на принадлежность к миэту по участникам
           Возвращает массив json с информацией о пользователях групп */
        internal static JArray LocalMietGroupCheck(List<string> local_groups_ids) 
        {
            List<List<string>> splited_local_groups_ids = new List<List<string>>();
            for (int i = 0; i < local_groups_ids.Count; i += 64)
            {
                splited_local_groups_ids.Add(local_groups_ids.GetRange(i, Math.Min(64, local_groups_ids.Count - i)));
            }

            List<string> unique_ids = new List<string>();
            JArray users_data = new JArray();
            Parallel.ForEach(splited_local_groups_ids, part_local_group_id =>
            {
                foreach (string local_group_id in part_local_group_id)
                { 
                    JArray group_data = VkApiAdvanced.LocalMietGroupCheck(local_group_id);
                    if (group_data != null)
                    {
                        for (int i = 0; i < group_data.Count; i++)
                        {
                            string id = group_data[i]["id"].ToString();
                            if (!unique_ids.Contains(id))
                            {
                                unique_ids.Add(id);
                                lock (users_data) users_data.Add(group_data[i]);
                            }
                        }
                    }
                }
            });

            return users_data;
        }
    }
}
