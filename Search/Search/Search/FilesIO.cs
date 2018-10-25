using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    // Класс для загрузки/сохранения файлов
    abstract class FilesIO
    {
        // TODO Генерировать уникальный идентификатор 
        private const string postfix = "_24_10_2018";

        // TODO Разобраться куда запихнуть сохранение/загрузку файлов
        /* IO */
        internal static void SaveFile(string filename, List<int> list)
        {
            File.WriteAllText("data/" + filename + postfix, String.Join(",", list.ToArray()));
            Console.WriteLine(filename + " was saved!");
        }

        internal static void SaveFile(string filename, List<string> list)
        {
            File.WriteAllText("data/" + filename + postfix, String.Join(",", list.ToArray()));
            Console.WriteLine(filename + " was saved!");
        }

        internal static void SaveFileJson(string filename, JArray data)
        {
            using (StreamWriter file = File.CreateText("data/" + filename + postfix + ".json"))
            {
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    data.WriteTo(writer);
                }
            }
            Console.WriteLine(filename + " was saved!");
        }
        /*
        public static List<int> LoadFile(string filename)
        {
            string file = File.ReadAllText("data/" + filename + postfix);
            List<int> list = new List<int>();
            foreach (string value in file.Split(','))
            {
                list.Add(int.Parse(value));
            }
            Console.WriteLine(filename + " was loaded!");
            return list;
        }
        */
        internal static List<string> LoadFile(string filename)
        {
            string file = File.ReadAllText("data/" + filename + postfix);
            List<string> list = new List<string>();
            foreach (string value in file.Split(','))
            {
                list.Add(value);
            }
            Console.WriteLine(filename + " was loaded!");
            return list;
        }

        internal static JArray LoadFileJson(string filename)
        {
            JArray data = new JArray();
            using (StreamReader file = File.OpenText("data/" + filename + postfix + ".json"))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    data = JArray.Load(reader);
                }
            }
            Console.WriteLine(filename + " was loaded!");
            return data;
        }
    }
}
