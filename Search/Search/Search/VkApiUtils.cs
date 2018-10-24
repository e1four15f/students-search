using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace Search
{
    // Класс, в котором хранятся вспомогательные методы и поля для vk api
    abstract class VkApiUtils
    {
        // ID приложения
        public readonly static string client_id = ConfigurationManager.AppSettings.Get("client_id");
        // Защищённый ключ
        public readonly static string client_secret = ConfigurationManager.AppSettings.Get("client_secret");
        // Сервисный ключ доступа
        public readonly static string service_key = ConfigurationManager.AppSettings.Get("service_key");
        // Версия API
        public readonly static string version = ConfigurationManager.AppSettings.Get("version");

        // Пользовательский ключ доступа
        public static string user_key = ConfigurationManager.AppSettings.Get("user_key");

        // Время засыпания потока при большом количестве запросов
        public const int sleep_time = 10000; 
        // Для вывода доп информации в консоль 
        private const bool verbose = true;

        // Метод для проверки user_key
        internal static void Auth()
        {
            bool auth = false;
            do
            {
                Console.WriteLine("Авторизация...");
                try
                {
                    VkApi.UsersSearch("Павел Дуров", 1);
                    auth = true;
                    Console.WriteLine("Успешно!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Пройдите по ссылке");
                    Console.WriteLine("https://oauth.vk.com/authorize?client_id=" + client_id + "&display=page&redirect_uri=http://vk.com&scope=offline&response_type=token&v=" + version);
                    Console.Write("Введите user_key: ");
                    user_key = Console.ReadLine();

                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["user_key"].Value = user_key;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    auth = false;
                }
            } while (!auth);
        }

        // Метод для посылки запроса по url
        private static string SendRequest(string url) 
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        // Мб придётся юзать такой асинхронный метод 
        private static async Task<string> SendRequestAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync()) 
            {
                using (Stream stream = response.GetResponseStream()) 
                {
                    using (StreamReader reader = new StreamReader(stream)) 
                    {  
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        // Метод для запроса метода в vk api
        private static short count = 0;
        internal static JObject MethodRequest(string method_name, Dictionary<string, string> parameters, string access_token) //, string version = version // TODO Нельзя задавать дефолтные значения, которые не являются const
        {
            string url_parameters = string.Empty;
            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                url_parameters += kvp.Key + '=' + kvp.Value + '&';
            }
            url_parameters = url_parameters.Remove(url_parameters.Length - 1);

            string response = SendRequest(@"https://api.vk.com/method/" + method_name + "?" + url_parameters + "&lang=0&access_token=" + access_token + "&v=" + version);
            
            // TODO Нормальный вывод загрузки
            if (verbose)
            {
                if (count % 10 == 0)
                {
                    Console.Write('#');
                    count = 0;
                }
                Console.Write(count);
                count++;
                Console.Write("\b");
            }
            
            JObject data = JObject.Parse(response);
            if (data["error"] != null)
            {
                string error_msg = data["error"]["error_msg"].ToString();
                short error_code = (short) data["error"]["error_code"];
                if (error_code == 6)
                {
                    Thread.Sleep(sleep_time);
                }
                Exception e = new Exception(string.Format("{0} : {1}", error_code, error_msg));
                Console.WriteLine(e.Message);
                e.Data.Add(error_code, error_msg);
                throw e;
            }

            if (verbose)
            { 
            //    Console.WriteLine(method_name);// + '\n' + data + '\n');
            }
            return data;
        }
    }
}
