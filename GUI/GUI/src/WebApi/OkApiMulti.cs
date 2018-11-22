using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi
{
    // Класс с распаралеленными методами
    abstract class OkApiMulti
    {
        /* Методы из ok api */

        /* Метод позволяющий получить подробную информацию о пользователях по листу string uid
           Возвращает массив json с информацией всех пользователей */
        internal static JArray UsersGetInfo(List<string> uids, List<string> fields = null)
        {
            JArray users_data = new JArray();

            Parallel.For(0, uids.Count / 100 + 1, i =>
            {
                JArray ranged_users_data = OkApi.UsersGetInfo(uids.GetRange(i * 100, i * 100 + 100 > uids.Count ? uids.Count - i * 100 : 100), fields);
                for (int j = 0; j < ranged_users_data.Count; j++)
                {
                    lock (users_data) users_data.Add(ranged_users_data[j]);
                }
            });

            return users_data;
        }

        /* Метод позволяющий получить подробную информацию о пользователях в комьюните по листу string gid
           Возвращает массив json с информацией всех пользователей */
        internal static JArray CommunitiesGetMembers(List<string> gids, List<string> fields = null, int start_year = 1910, int end_year = 2026, int count = 1000)
        {
            JArray users_data = new JArray();
            List<string> unique_uids = new List<string>();

            foreach (string gid in gids)
            {
                Parallel.ForEach(OkApiUtils.filters, filter =>
                {
                    for (int i = start_year; i < end_year; i += 2)
                    {
                        JArray ranged_users_data = OkApi.CommunitiesGetMembers(gid, fields, i, i + 1, filter);
                        for (int j = 0; j < ranged_users_data.Count; j++)
                        {
                            string uid = ranged_users_data[j]["uid"].ToString();
                            if (!unique_uids.Contains(uid))
                            {
                                lock (unique_uids) unique_uids.Add(uid);
                                lock (users_data) users_data.Add(ranged_users_data[j]);
                            }
                        }
                    }
                });
            }
            return users_data;
        }

        /* Метод позволяющий получить uid пользователей из листа string uids
           Возвращает лист string с uid всех пользователей */
        internal static List<string> GroupGetMembers(List<string> gids, int count = 1000)
        {
            List<string> unique_uids = new List<string>();

            foreach (string gid in gids)
            {
                Parallel.ForEach(OkApiUtils.direction, direction =>
                {
                    JArray ranged_users_data = OkApi.GroupGetMembers(gid, direction);
                    for (int j = 0; j < ranged_users_data.Count; j++)
                    {
                        string uid = ranged_users_data[j]["userId"].ToString();
                        if (!unique_uids.Contains(uid))
                        {
                            lock (unique_uids) unique_uids.Add(uid);
                        }
                    }

                });
            }
            return unique_uids;
        }
    }
}
