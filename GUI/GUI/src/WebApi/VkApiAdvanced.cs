using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApi
{
    // Класс, в котором хранятся продвинутые методы, использующие методы из vk api
    abstract class VkApiAdvanced
    {
        /* Метод проверяющий скрытую группу на принадлежность к миэту по участникам
         * Группа считается миэтовской, если в группе, где меньше 75 человек найдётся 8 человек из миэта
           Возвращает информацию об участниках, если она миэтовская */
        internal static JArray LocalMietGroupCheck(string local_group_id)
        {
            byte pivot = 0;
            List<string> user_ids = VkApi.GroupsGetMembers(local_group_id, 100);
            if (user_ids.Count > 75)
            {
                return null;
            }
            JArray users_data = VkApi.UsersGet(user_ids, VkApiUtils.fields, local_group_id);
                
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

            return null;
        }

        /* Метод позволюющий получить информацию об участниках группы 
           Возвращает информацию об участниках */
        internal static JArray GroupsGetMembers(string local_group_id, int max_count = 0)
        {
            List<string> user_ids = VkApi.GroupsGetMembers(local_group_id, max_count);
            JArray users_data = VkApiMulti.UsersGet(user_ids, VkApiUtils.fields, local_group_id);
            return users_data;
        }
    }
}
