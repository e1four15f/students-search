using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search
{
    // Класс, в котором хранятся продвинутые методы, использующие методы из vk api
    abstract class VkApiAdvanced
    {
        /* Метод проверяющий скрытую группу на принадлежность к миэту по участникам
         * Группа считается миэтовской, если в группе, где меньше 75 человек найдётся 5 человек из миэта
           Возвращает информацию об участниках, если она миэтовская */
        internal static JArray LocalMietGroupCheck(string local_group_id)
        {
            byte pivot = 0;
            List<string> user_ids = VkApi.GroupsGetMembers(local_group_id, 100);
            if (user_ids.Count > 75)
            {
                // TODO мб возвращать пустую строку или null
                return null;
            }
            // TODO Добавить в токен поле from group_id
            JArray users_data = VkApi.UsersGet(user_ids, VkApiUtils.fields);
            foreach (JToken user_data in users_data)
            {
                // TODO Определить оптимальное значение для pivot
                if (pivot == 8)
                {
                    return users_data;
                }
                
                if (user_data["occupation"] != null && (string) user_data["occupation"]["id"] == "241")
                {
                    pivot++;
                    continue;
                }

                if (user_data["universities"] != null) 
                { 
                    foreach (JToken university in user_data["universities"])
                    {
                        if ((string) university["id"] == "241")
                        {
                            pivot++;
                            break;
                        }
                    }
                }
            }

            // TODO мб возвращать пустую строку или null
            return null;
        }

        /* В данный момент не используется
         * Метод для нахождения скрытых групп по строке запроса
           Возвращает лист int с id скрытых групп 
        internal static List<int> GetHiddenGroups(string word, short count = 1000)
        {
            HashSet<int> hidden_group_ids = new HashSet<int>();

            List<int> group_ids = VkApi.GroupsSearch(word, count);
            for (int i = 0; i < group_ids.Count; i += 4)
            {
                List<int> part_group_ids = group_ids.GetRange(i, Math.Min(4, group_ids.Count - i));
                if (part_group_ids.Count > 0)
                {
                    foreach (int group_id in VkApi.GroupsGetById(part_group_ids))
                    {
                        hidden_group_ids.Add(group_id);
                    }
                }
            } 

            return hidden_group_ids.ToList<int>();
        }
        */
    }
}
