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
                return null;
            }
            JArray users_data = VkApi.UsersGet(user_ids, VkApiUtils.fields);

            for (int i = 0; i < users_data.Count; i++)
            {
                if (users_data[i]["deactivated"] != null || users_data[i].Type == JTokenType.Null)
                {
                    users_data.RemoveAt(i);
                }
                else
                {
                    users_data[i]["arrived_from"] = local_group_id;
                    if (users_data[i]["occupation"] != null && (string) users_data[i]["occupation"]["id"] == "241")
                    {
                        pivot++;
                        continue;
                    }

                    if (users_data[i]["universities"] != null)
                    {
                        foreach (JToken university in users_data[i]["universities"])
                        {
                            if ((string) university["id"] == "241")
                            {
                                pivot++;
                                break;
                            }
                        }
                    }
                }
            }
            
            // TODO Определить оптимальное значение для pivot
            if (pivot == 8)
            {
                return users_data;
            }
            
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
