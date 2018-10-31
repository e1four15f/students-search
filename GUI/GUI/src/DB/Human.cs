using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DB
{
    // TODO Нужно ли в структурах хранить и id, и название?
    // TODO Разобраться как хранить переменные 
    public struct City
    {
        public int city_id;
        public string city_title; 
    }

    public struct Contacts
    {
        public string mobile_phone;
        public string home_phone;
        public string email;
    }

    public struct Social
    {
        public string skype;
        public string facebook;
        public string instagram;
        public string livejournal;
        public string twitter;
    }

    public struct University
    {
        public int university_id;
        public string university_name;
        public int faculty_id;
        public string faculty_name;
        public int chair_id;
        public string chair_name;
        public int graduation_year;
    }

    public class Human
    {
        public int id; // TODO Если будет несколько сетей, то можно запутаться в ID
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool sex; 
        public string Sex { get { return sex ? "Мужчина" : "Женщина"; } }
        public string domain { get; set; }
        public DateTime bdate { get; set; }
        public string BDate { get { return bdate != new DateTime() ? bdate.Year > DateTime.MinValue.Year ? bdate.ToShortDateString() : bdate.Day + "." + bdate.Month : ""; } }

        public City city;
        public string city_title { get { return city.city_title; } }

        public Contacts contacts;
        public string mobile_phone { get { return contacts.mobile_phone != null ? "Моб. " + contacts.mobile_phone : ""; } }
        public string home_phone { get { return contacts.home_phone != null ? "Дом. " + contacts.home_phone : ""; } }
        public string email { get { return contacts.email; } }

        public Social social; // TODO // TODO Odnoklassniki
        public string skype { get { return social.skype != null ? "skype:" + social.skype + "?call" : null; } }
        public string facebook { get { return social.facebook != null ? "https://www.facebook.com/" + social.facebook : null; } } 
        public string instagram { get { return social.instagram != null ? "https://www.instagram.com/" + social.instagram : null; } }
        public string livejournal { get { return social.livejournal != null ? "https://" + social.livejournal + ".livejournal.com/" : null; } }
        public string twitter { get { return social.twitter != null ? "https://twitter.com/" + social.twitter : null; } }
        public string vk { get { return "https://vk.com/" + domain; } } // TODO Потом вк не всегда будет 

        // TODO Показывается первый университет в списке, в нём не всегда полная информация 
        public List<University> universities { get; set; }
        public string university_name { get { return universities.Count > 0 ? universities[0].university_name : ""; } }
        public string faculty_name { get { return universities.Count > 0 ? universities[0].faculty_name : ""; } }
        public string chair_name { get { return universities.Count > 0 ? universities[0].chair_name : ""; } }
        public string graduation_year { get { return (universities.Count > 0 && universities[0].graduation_year > 0) ? "Год окончания " + universities[0].graduation_year : ""; } }

        public string photo_100 { get; set; }
        public string arrived_from { get; set; }

        public int plausibility { get; set; }

        public Human(JToken user_data)
        {
            id = (int) user_data["id"];
            first_name = user_data["first_name"].ToString();
            last_name = user_data["last_name"].ToString();
            sex = (int) user_data["sex"] == 2 ? true : false;
            domain = user_data["domain"].ToString();

            if (user_data["bdate"] != null)
            {
                int[] splited_date = user_data["bdate"].ToString().Split('.').Select(Int32.Parse).ToArray();
                {
                    try
                    {
                        bdate = (splited_date.Length == 3) ? new DateTime(splited_date[2], splited_date[1], splited_date[0]) : new DateTime(DateTime.MinValue.Year, splited_date[1], splited_date[0]);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Невозможно получить год рождения id:" + user_data["id"] + " bdate:" + user_data["bdate"]);
                    }
                }
            }

            if (user_data["city"] != null)
            {
                city.city_id = (int) user_data["city"]["id"];
                city.city_title = user_data["city"]["title"].ToString();
            }

            contacts.mobile_phone = user_data["mobile_phone"] != null && user_data["mobile_phone"].ToString().Length > 8 
                ? Regex.Replace(user_data["mobile_phone"].ToString(), @"\t|\n|\r", "") : null;
            contacts.home_phone = user_data["home_phone"] != null && user_data["home_phone"].ToString().Length > 8
                ? Regex.Replace(user_data["home_phone"].ToString(), @"\t|\n|\r", "") : null;
            contacts.email = user_data["email"] != null ? user_data["email"].ToString() : null;

            social.instagram = user_data["instagram"] != null ? user_data["instagram"].ToString() : null;
            social.skype = user_data["skype"] != null ? user_data["skype"].ToString() : null;
            social.facebook = user_data["facebook"] != null ? user_data["facebook"].ToString() : null;
            social.livejournal = user_data["livejournal"] != null ? user_data["livejournal"].ToString() : null;
            social.twitter = user_data["twitter"] != null ? user_data["twitter"].ToString() : null;

            photo_100 = user_data["photo_100"].ToString();

            universities = new List<University>();

            if (user_data["universities"] != null)
            {
                foreach (JToken university_data in user_data["universities"])
                {
                    if (university_data.Type != JTokenType.Null)
                    {
                        AddUniversity(university_data);
                    }
                }
            }

            if (user_data["university"] != null)
            {
                AddUniversity(user_data);
            }

            if (user_data["occupation"] != null && user_data["occupation"]["type"].ToString() == "university")
            {
                University occupation = new University();
                occupation.university_id = (int) user_data["occupation"]["id"];
                occupation.university_name = user_data["occupation"]["name"].ToString();
                universities.Add(occupation);
            }

            arrived_from = user_data["arrived_from"].ToString();
        }

        private void AddUniversity(JToken university_data)
        {
            University university = new University();
            if (university_data["university"] == null)
            {
                university.university_id = (int) university_data["id"];
                university.university_name = university_data["name"].ToString();
            }
            else
            {
                university.university_id = (int) university_data["university"];
                university.university_name = university_data["university_name"].ToString();
            }
            
            if (university_data["faculty"] != null)
            {
                university.faculty_id = (int)university_data["faculty"];
                university.faculty_name = Regex.Replace(university_data["faculty_name"].ToString(), @"\t|\n|\r", "");
            }
            if (university_data["chair"] != null)
            {
                university.chair_id = (int) university_data["chair"];
                university.chair_name = Regex.Replace(university_data["chair_name"].ToString(), @"\t|\n|\r", "");
            }
            university.graduation_year = university_data["graduation"] != null ? int.Parse(university_data["graduation"].ToString()) : 0;
            universities.Add(university);
        }

        // TODO quick math
        public int CalcPlausibility()
        {
            return 100;
        }

        public override string ToString()
        {
            string user_info = domain + " " + first_name + " " + last_name + " " + (sex ? "Муж" : "Жен") + " " + bdate.ToShortDateString() + " " + city.city_title + " " + arrived_from + "\n"
                + (contacts.mobile_phone != null ? " Моб.телефон:" + contacts.mobile_phone : "") + (contacts.home_phone != null ? " Дом.телефон:" + contacts.home_phone : "")
                + (social.instagram != null ? " instagram:" + social.instagram : "")
                + (social.skype != null ? " skype:" + social.skype : "")
                + (social.facebook != null ? " facebook:" + social.facebook : "")
                + (social.livejournal != null ? " livejournal:" + social.livejournal : "")
                + (social.twitter != null ? " twitter:" + social.twitter : "")
                + "\n";

            foreach (University university in universities)
            {
                user_info += university.university_name + " " 
                    + university.faculty_name + " " 
                    + (university.graduation_year != 0 ? university.graduation_year.ToString() : "") + "\n";
            }
            return user_info;
        }
    }
}
