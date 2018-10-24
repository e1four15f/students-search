using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Search
{
    // Класс, в котором хранятся методы из vk api
    abstract class VkApi
    {
        /* Метод позволяющий получить подробную информацию о пользователях по листу string с id
           Возвращает массив json с информацией пользователей */
        internal static JArray UsersGet(List<string> user_ids, string fields = "")
        {
            JArray users_data = new JArray();
            
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "user_ids", String.Join(",", String.Join(",", user_ids)) },
                { "fields", fields }
            };

            JObject data = new JObject();
            try
            {
                data = VkApiUtils.MethodRequest("users.get", parameters, VkApiUtils.service_key);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message + "\nПревышено количество пользователей, должно быть <= 350");
            }
            
            return (JArray) data["response"];
        }

        /* Метод позволяющий выполнить поиск групп по текстовой строке
           Возвращает лист int с id групп */
        internal static List<int> GroupsSearch(string text, short count = 1000)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "q", text },
                { "type", "groups" },
                { "count", count.ToString() }
            };
            JObject data;
            try
            {
                data = VkApiUtils.MethodRequest("groups.search", parameters, VkApiUtils.user_key);
            }
            catch (Exception e)
            {
                if (e.Data.Keys.Cast<short>().Single() == 6)
                {
                    return GroupsSearch(text, count);
                }
                else
                { 
                    throw e;
                }
            }
            
            List<int> group_ids = new List<int>();
            foreach (JToken group in data["response"]["items"])
            {
                group_ids.Add((int) group["id"]);
            }

            return group_ids;
        }

        /* Метод позволяющий получить всех пользователей группы по id
           Возвращает лист int с id пользователей */
        internal static List<string> GroupsGetMembers(string group_id, int max_count = 0)
        {
            short offset = 0;
            List<string> user_ids = new List<string>();

            while (max_count == 0 || user_ids.Count <= max_count)
            {
                if (group_id.Length > 6 && group_id.Substring(0, group_id.Length - 5).Contains("club") && group_id.Substring(4).All(char.IsDigit))
                {
                    group_id = group_id.Substring(4);
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>()
                {
                    { "group_id", group_id },
                    { "count", "1000" },
                    { "offset", (1000*offset).ToString() }
                };
                JObject data;
                try
                {
                    data = VkApiUtils.MethodRequest("groups.getMembers", parameters, VkApiUtils.service_key);
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    return new List<string>();
                }
                catch (Exception e)
                {
                    if (e.Data.Keys.Cast<short>().Single() == 6)
                    {
                        return GroupsGetMembers(group_id, max_count);
                    }
                    else
                    {
                        throw e;
                    }
                } 

                foreach (JToken user_id in data["response"]["items"])
                {
                    user_ids.Add(user_id.ToString());
                }

                if (data["response"]["items"].Count() < 1000)
                {
                    return user_ids;
                }
                offset++;
            }
            
            return user_ids.GetRange(0, max_count);
        }

        /* В данный момент не используется
         * Метод позволяющий получить информацию о группах по id
         * Мы используем для определения, что группа скрытая
           Возвращает лист int с id скрытых групп
        internal static List<int> GroupsGetById(List<int> group_ids)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "group_ids", String.Join(",", group_ids) },
                { "type", "group" }
            };
            JObject data;
            try
            {
                data = VkApiUtils.MethodRequest("groups.getById", parameters, VkApiUtils.service_key);
            }
            catch (Exception e)
            {
                if (e.Data.Keys.Cast<short>().Single() == 6)
                {
                    return GroupsGetById(group_ids);
                }
                else
                {
                    throw e;
                }
            }

            List<int> checked_group_ids = new List<int>();
            foreach (JToken group in data["response"])
            {
                if ((int) group["is_closed"] == 1 && group["deactivated"] == null)
                {
                    checked_group_ids.Add((int) group["id"]);
                }
            }

            return checked_group_ids;
        }
        */

        /* Метод позволяющий произвести поиск людей по параметрам
           Возвращает лист int с id найденных пользователей */
        internal static List<string> UsersSearch(string q = "", short count = 1000, short age = 0, byte sex = 0)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "q", q },
                { "count", count.ToString() },
                { "university", "241" },
                { "sex", sex.ToString() },
                // TODO Сейчас age задаёт определённый возраст, мб лучше сделать рендж?
                { "age_from", age.ToString() },
                { "age_to", age.ToString() }
            };
            JObject data;
            try
            {
                data = VkApiUtils.MethodRequest("users.search", parameters, VkApiUtils.user_key);
            }
            catch (Exception e)
            {
                if (e.Data.Keys.Cast<short>().Single() == 6)
                {
                    return UsersSearch(q, count, age, sex);
                }
                else
                {
                    throw e;
                }
            }

            List<string> user_ids = new List<string>();
            foreach (JToken user in data["response"]["items"])
            {
                user_ids.Add(user["id"].ToString());
            }

            return user_ids;
        }
    }
}
