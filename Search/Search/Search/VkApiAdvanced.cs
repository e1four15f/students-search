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
           Возвращает id группы, если она миэтовская */
        internal static string HiddenMietGroupCheck(string hidden_group_id)
        {
            byte pivot = 0;
            List<int> data = VkApi.GroupsGetMembers(hidden_group_id, 100);
            if (data.Count > 75)
            {
                // TODO мб возвращать пустую строку или null
                return null;
            }
            
            foreach (int user_id in data)
            {
                if (pivot == 5)
                {
                    return hidden_group_id;
                }

                JToken user_data = VkApi.UsersGet(user_id, "occupation,universities");
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

        /* Метод для нахождения скрытых групп по строке запроса
           Возвращает лист int с id скрытых групп */
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
    }
}
