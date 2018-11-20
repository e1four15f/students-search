using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

// TODO Мб замутить OkApi
namespace WebApi
{
    // Класс, в котором хранятся методы из ok api
    abstract class OkApi
    {
        /* Метод позволяющий получить подробную информацию о пользователях по листу string с uid
           Возвращает массив json с информацией пользователей */
        internal static JArray UsersGetInfo(List<string> uids, List<string> fields = null)
        {
            if (uids.Count() > 100)
            {
                Console.WriteLine("Превышено количество пользователей");
                return null;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "fields", String.Join(",", fields) },
                { "format", "json" },
                { "method", "users.getInfo" },
                { "uids", String.Join(",", uids) }
            };

            JToken data = OkApiUtils.MethodRequest(parameters);
            return (JArray)data;
        }

        /* Метод позволяющий получить подробную информацию о пользователях из комьюнити по string с gid
           Возвращает массив json с информацией пользователей */
        internal static JArray CommunitiesGetMembers(string gid, List<string> fields = null, int start_year = 1910, int end_year = 2026, string filter = "DEFAULT", int count = 1000)
        {
            if (count > 1000)
            {
                Console.WriteLine("Превышено количество пользователей");
                return null;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "count", count.ToString() },
                { "end_year", end_year.ToString() },
                { "fields", String.Join(",", fields) },
                { "filter", filter },
                { "format", "json" },
                { "gid",  gid },
                { "method", "communities.getMembers" },
                { "start_year", start_year.ToString() }
            };

            JArray data = (JArray)OkApiUtils.MethodRequest(parameters)["users"];
            return data;
        }

        /* Метод позволяющий получить uid пользователей из группы по string с gid
           Возвращает массив json с uid пользователей */
        internal static JArray GroupGetMembers(string uid, string direction = "AROUND", int count = 1000)
        {
            if (count > 1000)
            {
                Console.WriteLine("Превышено количество пользователей");
                return null;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "count", count.ToString() },
                { "direction", direction },
                { "format", "json" },
                { "method", "group.getMembers" },
                { "uid",  uid }
            };

            JArray data = (JArray)OkApiUtils.MethodRequest(parameters)["members"];
            return data;
        }
    }
}
