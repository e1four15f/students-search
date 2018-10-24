using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    // Класс с распаралеленными методами
    abstract class VkApiMulti
    {
        /* Методы из vk api */

        /* Метод позволяющий получить подробную информацию о пользователях в листе int по id
           Возвращает лист json с информацией всех пользователей */
        internal static JArray UsersGet(List<int> user_ids, string fields = "") 
        {
            JArray users_data = new JArray();
            Parallel.ForEach(user_ids, user_id =>
            {
                users_data.Add(VkApi.UsersGet(user_id, fields));
            });
            return users_data;
        }

        /* Метод позволяющий выполнить поиск групп по текстовым строкам из листа string
           Возвращает лист int с id групп */
        internal static List<int> GroupsSearch(List<string> texts, short count = 1000) 
        {
            HashSet<int> group_ids = new HashSet<int>();
            Parallel.ForEach(texts, text =>
            {
                foreach (int group_id in VkApi.GroupsSearch(text, count))
                {
                    group_ids.Add(group_id);
                }
            });
            return group_ids.ToList<int>();
        }

        /* Метод позволяющий получить всех пользователей из групп в листе string
           Возвращает лист int с id всех пользователей */
        internal static List<int> GroupsGetMembers(List<string> group_ids, int max_count = 0) 
        {
            HashSet<int> user_ids = new HashSet<int>();
            Parallel.ForEach(group_ids, group_id =>
            {
                foreach (int user_id in VkApi.GroupsGetMembers(group_id, max_count))
                {
                    user_ids.Add(user_id);
                }
            });
            return user_ids.ToList<int>();
        }

        /* Метод позволяющий произвести поиск людей по всем параметрам
           Возвращает лист int с id найденных пользователей */
        // TODO Задавать age=range(0, 120), sex=range(1, 3)
        internal static List<int> UsersSearch(string q = "", short count = 1000, short age_min = 0, short age_max = 120) //, List<short> age = 0, byte sex = 0)
        {
            HashSet<int> user_ids = new HashSet<int>();
            Parallel.For(age_min, age_max, age =>
            {
                foreach (int user_id in VkApi.UsersSearch(q, count, (short) age, 1))
                {
                    user_ids.Add(user_id);
                }
                foreach (int user_id in VkApi.UsersSearch(q, count, (short)age, 2))
                {
                    user_ids.Add(user_id);
                }
            });
            return user_ids.ToList<int>();
        }

        /* Методы из vk api advanced */

        /* Метод проверяющий скрытые группы из листа string на принадлежность к миэту по участникам
           Возвращает лист string id миэтовских групп */
        internal static List<string> HiddenMietGroupCheck(List<string> hidden_groups_ids) 
        {
            List<string> group_ids = new List<string>();
            Parallel.ForEach(hidden_groups_ids, hidden_group_id =>
            {
                string group_id = VkApiAdvanced.HiddenMietGroupCheck(hidden_group_id);
                if (group_id != null)
                { 
                    group_ids.Add(group_id);
                }
            });
            return group_ids;
        }

        /* Метод для нахождения скрытых групп по текстовым строкам из листа string
           Возвращает лист int с id скрытых групп */
        internal static List<int> GetHiddenGroups(List<string> words, short count = 1000)
        {
            HashSet<int> group_ids = new HashSet<int>();
            Parallel.ForEach(words, word =>
            {
                foreach (int group_id in VkApiAdvanced.GetHiddenGroups(word, count))
                {
                    group_ids.Add(group_id);
                }
            });
            return group_ids.ToList<int>();
        }
    }
    /*
        static List<JToken> Multi(Func<int, JToken> method, List<int> list)
        {
            List<JToken> new_user_ids = new List<JToken>();
            Parallel.For(0, list.Count, i =>
            {
                new_user_ids.Add(method(list.ElementAt(i)));
            });
            return new_user_ids;
        }
        */
}
