using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace WebApi
{
    abstract class OkApiUtils
    {
        // https://connect.ok.ru/oauth/authorize?client_id=1273054720&scope=VALUABLE_ACCESS;LONG_ACCESS_TOKEN&response_type=code&redirect_uri=https://apiok.ru/oauth_callback
        // https://api.ok.ru/oauth/token.do?client_id=1273054720&client_secret=C54EF013424CD0A09D8EB453&redirect_uri=https://apiok.ru/oauth_callback&grant_type=authorization_code&code=1goMfEfRuJAEx1H2ANs3ZyPWGtYWJcjf9PmcvF7EGj7Osu9b5zLW8Zi3D6QV9rs69c0AMFQLzeaOQi9HZ7ViYjZdwQhQtwXymOfCBWBZSwSyPIV8fjINoiP7qFiT2qb2FF48uqVcwfvUUTyNuBuSu9yqJvQmou2dQShyzZmAE9h1uj

        // Данные пользователя
        private static string code = "1Uga4YjYzWA1RxgTSh1gxmpIQo0VqWNoyH3H2dFREACnNQqhe87snaKx8ajNpa6pH61mw73wFQIemSWqixtUWBiQjTAVn2OW7cO7kVZZSVF0UIHgAHWXpS32zavHkOc9QxvsuXO748ejOvfcLugxljP9b0lJwN4HYNx75TY1yx7P1";
        private static string access_token = "-s-1RRuPBejrTmVWA4B7cnaX8he7zkbVFqC6wP-SDle6z-Tb";
        private static string refresh_token = "d19050552451_f517ab9687f37b0d761df1b4143a3607_57801501996c";

        // Данные о приложении 
        public static string application_id = "1273054720";
        public static string application_key = "CBAHECOMEBABABABA";
        public static string application_secret_key = "C54EF013424CD0A09D8EB453";

        public readonly static List<string> fields = new List<string>() 
        {
            "BIRTHDAY",
            "CITY_OF_BIRTH",
            "CURRENT_LOCATION",
            "EMAIL",
            "FIRST_NAME",
            "GENDER",
            "LAST_NAME",
            "LOCATION",
            "PIC128X128",
        };

        public readonly static List<string> filters = new List<string>()
        {
            "FINISH",
            "GRADUATION",
            "STUDY" 
        };

        public readonly static List<string> direction = new List<string>()
        {
            "AROUND",
            "BACKWARD",
            "FORWARD"
        };

        public readonly static List<string> communities = new List<string>() 
        { 
            "6121697957", 
            "11089606213748", 
            "14254787723445"
        };

        public readonly static List<string> open_groups = new List<string>() 
        { 
            "54509151649807", 
            "44343278960713", 
            "53649854758919", 
            "50407879933968", 
            "50569067102360", 
            "54367354814464", 
            "50569067102360", 
            "52705710440624", 
            "48172980568090"
        };

        // Метод для запроса метода в ok api
        internal static JToken MethodRequest(Dictionary<string, string> parameters)
        {
            string secret_key = MD5Hash(access_token + application_secret_key);

            string sig_parameters = "application_key=" + application_key;
            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                sig_parameters += kvp.Key + "=" + kvp.Value;
            }
            string sig = MD5Hash(sig_parameters + secret_key);

            if (parameters.ContainsKey("fields"))
            {
                parameters["fields"] = parameters["fields"].Replace(",", "%2C");
            }

            string request = "https://api.ok.ru/fb.do?application_key=" + application_key;
            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                request += "&" + kvp.Key + "=" + kvp.Value;
            }
            request += "&sig=" + sig + "&access_token=" + access_token;

            string response = SendRequest(request);

            if (response.Contains("error_code"))
            {
                Console.WriteLine(response);
                return null;
            }

            JToken data = JToken.Parse(response);
            return data;
        }

        // Метод для посылки запроса по url
        private static string SendRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
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

        // Метод для md5 хеширования
        private static string MD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
    }
}
