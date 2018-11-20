using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using DB;

namespace Utils
{
    abstract class ReportGenerator
    {
        public static void GeneratePrintableList(string filename, List<Human> users)
        {
            string style = Path.GetDirectoryName(filename) + "\\style.css";
            using (var stream = File.Create(filename)) { };
            File.WriteAllText(style, GUI.Properties.Resources.style);

            File.WriteAllText(filename, GenerateHeader(Path.GetFileNameWithoutExtension(filename)));

            foreach (Human user in users)
            {
                File.AppendAllText(filename, GenerateHuman(user));
            }

            File.AppendAllText(filename, "\n</body>\n</html>");
        }

        private static string GenerateHeader(string list_name)
        {
            string header = GUI.Properties.Resources.header;
            return Regex.Replace(header, "{list_name}", list_name);
        }

        private static string GenerateHuman(Human user)
        {
            string content = GUI.Properties.Resources.content;

            content = Regex.Replace(content, "{photo_100}", user.photo_100);

            content = Regex.Replace(content, "{first_name}", user.first_name);
            content = Regex.Replace(content, "{last_name}", user.last_name);
            content = Regex.Replace(content, "{bdate}", user.BDate);
            content = Regex.Replace(content, "{sex}", user.Sex);

            content = Regex.Replace(content, "{university}", user.university_name != null ? user.university_name : "");
            content = Regex.Replace(content, "{faculty}", user.faculty_name != null ? user.faculty_name : "");
            content = Regex.Replace(content, "{chair}", user.chair_name != null ? user.chair_name : "");
            content = Regex.Replace(content, "{city}", user.city_title != null ? user.city_title : "");
            content = Regex.Replace(content, "{addr}", user.vk != null ? user.vk : user.ok);

            content = Regex.Replace(content, "{mobile_phone}", user.mobile_phone != null ? user.mobile_phone : "");
            content = Regex.Replace(content, "{home_phone}", user.home_phone != null ? user.home_phone : "");

            return content;
        }
    }
}
