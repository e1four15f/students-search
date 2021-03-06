﻿using Newtonsoft.Json.Linq;
using LiteDB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

using Utils;

namespace DB
{
    // TODO Нужно ли в структурах хранить и id, и название?
    // TODO Разобраться как хранить переменные 
    public class City
    {
        public int city_id { get; set; }
        public string city_title { get; set; }
    }

    public class Contacts
    {
        public string mobile_phone { get; set; }
        public string home_phone { get; set; }
        public HashSet<string> emails { get; set; }
        public HashSet<string> sites { get; set; }
    }

    public class Social
    {
        public string skype { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string livejournal { get; set; }
        public string twitter { get; set; }
    }

    public class University
    {
        public int university_id { get; set; }
        public string university_name { get; set; }
        public int faculty_id { get; set; }
        public string faculty_name { get; set; }
        public int chair_id { get; set; }
        public string chair_name { get; set; }
        public int graduation_year { get; set; }
    }

    delegate bool CheckField(dynamic for_check, ref int plausibility);
    public class Human
    {
        public ObjectId _id { get; set; }
        // TODO пришлось переименовать, litedb почему-то скипал и оставлял дефолтное значение 
        public int vk_id { get; set; }
        public string ok_id { get; set; } 
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool sex { get; set; }
        [BsonIgnore]
        public string Sex { get { return sex ? "Мужчина" : "Женщина"; } }
        public string domain { get; set; }
        public DateTime bdate { get; set; }
        [BsonIgnore]
        public string BDate { get { return bdate != new DateTime() ? bdate.Year > DateTime.MinValue.Year ? bdate.ToShortDateString() : bdate.Day + "." + bdate.Month : ""; } }

        public City city { get; set; }
        [BsonIgnore]
        public string city_title { get { return city.city_title; } }

        public Contacts contacts { get; set; }
        [BsonIgnore]
        public string mobile_phone { get { return contacts.mobile_phone != null ? "Моб. " + contacts.mobile_phone : ""; } }
        [BsonIgnore]
        public string home_phone { get { return contacts.home_phone != null ? "Дом. " + contacts.home_phone : ""; } }
        [BsonIgnore]
        public string email { get { return contacts.emails != null ? contacts.emails.ElementAtOrDefault(0) : ""; } }

        public Social social { get; set; } // TODO // TODO Odnoklassniki
        [BsonIgnore]
        public string skype { get { return social.skype != null ? "skype:" + social.skype + "?call" : null; } }
        [BsonIgnore]
        public string facebook { get { return social.facebook != null ? "https://www.facebook.com/" + social.facebook : null; } }
        [BsonIgnore]
        public string instagram { get { return social.instagram != null ? "https://www.instagram.com/" + social.instagram : null; } }
        [BsonIgnore]
        public string livejournal { get { return social.livejournal != null ? "https://" + social.livejournal + ".livejournal.com/" : null; } }
        [BsonIgnore]
        public string twitter { get { return social.twitter != null ? "https://twitter.com/" + social.twitter : null; } }
        [BsonIgnore]
        public string vk { get { return domain != null ? "https://vk.com/" + domain : null; } } 
        [BsonIgnore]
        public string ok { get { return ok_id != null ? "https://ok.ru/profile/" + ok_id : null; } } 

        // TODO Показывается первый университет в списке, в нём не всегда полная информация 
        public List<University> universities { get; set; }
        [BsonIgnore]
        public string university_name { get { return universities.Count > 0 ? universities[0].university_name : ""; } }
        [BsonIgnore]
        public string faculty_name { get { return universities.Count > 0 ? universities[0].faculty_name : ""; } }
        [BsonIgnore]
        public string chair_name { get { return universities.Count > 0 ? universities[0].chair_name : ""; } }
        [BsonIgnore]
        public string graduation_year { get { return (universities.Count > 0 && universities[0].graduation_year > 0) ? "Год окончания " + universities[0].graduation_year : ""; } }

        public string photo_100 { get; set; }
        public string arrived_from { get; set; }
        
        public int plausibility;
        public int Plausibility { get { return plausibility; } set { plausibility = value; } }
        
        //для замены большого числа if'ов
        [BsonIgnore]
        CheckField check_social = (dynamic for_check, ref int plausibility) =>
        {
            if (!String.IsNullOrEmpty(for_check))
                plausibility += 2;
            else
                plausibility--;
            return true;
        };
        
        //для замены большого if'а
        [BsonIgnore]
        CheckField check_university = (dynamic university, ref int plausibility) =>
        {
            if (university.university_id == 241)
            {
                plausibility += 15;

                if (university.faculty_id != 0)
                    plausibility += 3;

                if (university.chair_id != 0)
                    plausibility += 3;

                return false;
            }
            else
            {
                plausibility -= 8;
                 
                if (university.faculty_id != 0)
                    plausibility++;

                if (university.chair_id != 0)
                    plausibility++;
            }
        	return true;
        };
        
        
        [BsonIgnore]
        public string plausibility_color { get { return "#" + ((int)(6.375 * (40 - Plausibility))).ToString("X2") + ((int)(6.375 * Plausibility)).ToString("X2") + "00"; } }
        
        [BsonIgnore]
        public HashSet<Human> friends { get; set; }
        // Дефолтный конструктор нужен для бд
        public Human()
        {
            vk_id = 0;
            first_name = null;
            last_name = null;
            sex = false;
            domain = null;
            bdate = new DateTime();
            city = new City();
            social = new Social();
            contacts = new Contacts();
            photo_100 = null;
            arrived_from = null;
            Plausibility = 0;
        }

        // Контроллер данных соц сетей
        public Human(JToken user_data, string from)
        {
            if (from == "vk") FromVk(user_data);
            else if (from == "ok") FromOk(user_data);
        }

        // Обработчик данных из вк
        private void FromVk(JToken user_data)
        {
            vk_id = (int) user_data["id"];
            first_name = user_data["first_name"].ToString();
            last_name = user_data["last_name"].ToString();
            sex = (int) user_data["sex"] == 2 ? true : false;
            domain = user_data["domain"].ToString();

            city = new City();
            social = new Social();
            contacts = new Contacts();

            if (user_data["bdate"] != null)
            {
                int[] splited_date = user_data["bdate"].ToString().Split('.').Select(Int32.Parse).ToArray();
                {
                    try
                    {
                        bdate = (splited_date.Length == 3) ? new DateTime(splited_date[2], splited_date[1], splited_date[0]) : new DateTime(DateTime.MinValue.Year, splited_date[1], splited_date[0]);
                    }
                    catch (ArgumentOutOfRangeException)
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

            contacts.emails = new HashSet<string>();
            contacts.emails.Add(user_data["email"] != null ? user_data["email"].ToString() : null);

            contacts.sites = new HashSet<string>();

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
            // Вычисляем плюсабилити 
            CalcPlausibility();
        }
        
        // Обработчик данных из ок
        private void FromOk(JToken user_data)
        {
            ok_id = user_data["uid"].ToString();
            first_name = user_data["first_name"].ToString();
            last_name = user_data["last_name"].ToString();
            sex = user_data["gender"].ToString() == "male" ? true : false;
            //domain = user_data["domain"].ToString();

            city = new City();
            social = new Social();
            contacts = new Contacts();

            if (user_data["birthday"] != null)
            {
                int[] splited_date = user_data["birthday"].ToString().Split('-').Select(Int32.Parse).ToArray();
                {
                    try
                    {
                        bdate = (splited_date.Length == 3) ? new DateTime(splited_date[0], splited_date[1], splited_date[2]) : new DateTime(DateTime.MinValue.Year, splited_date[0], splited_date[1]);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Console.WriteLine("Невозможно получить год рождения id:" + user_data["uid"] + " bdate:" + user_data["birthday"]);
                    }
                }
            }

            if (user_data["location"] != null)
            {
                city.city_id = -1;
                city.city_title = user_data["location"]["city"].ToString();
            }

            //contacts.mobile_phone = null;
            //contacts.home_phone = null;

            contacts.emails = new HashSet<string>();
            contacts.emails.Add(user_data["email"] != null ? user_data["email"].ToString() : null);

            contacts.sites = new HashSet<string>();

            //social.instagram = user_data["instagram"] != null ? user_data["instagram"].ToString() : null;
            //social.skype = user_data["skype"] != null ? user_data["skype"].ToString() : null;
            //social.facebook = user_data["facebook"] != null ? user_data["facebook"].ToString() : null;
            //social.livejournal = user_data["livejournal"] != null ? user_data["livejournal"].ToString() : null;
            //social.twitter = user_data["twitter"] != null ? user_data["twitter"].ToString() : null;

            if (user_data["pic128x128"].ToString() == "https://i.mycdn.me/res/stub_128x128.gif")
            {
                // TODO мб что-нибудь другое юзать
                photo_100 = "http://asiamountains-hotels.com/am2/wp-content/themes/asiamountainshotel/images/noavatar.png";
            }
            else
            {
                photo_100 = user_data["pic128x128"].ToString();
            }

            // TODO Пока поставим, что они из миэта
            universities = new List<University>();
            University university = new University();
            university.university_id = 241;
            university.university_name = "МИЭТ";
            universities.Add(university);

            arrived_from = "Ok";
            // Вычисляем плюсабилити 
            CalcPlausibility();
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

        //если внести graduation_year в Human, то надо убрать передачу аргумента
        public int CalcPlausibility()
        {
            Plausibility = 0;

            if (universities.Any())
            { 
                foreach (University university in universities)
                {
                    if (university.graduation_year != 0 && AnalyzeData.CheckAge(bdate.Year, university.graduation_year))
            	        Plausibility += 2;
                    
                    if(!check_university(university, ref plausibility))
                    	break;
                }
            }
            
            check_social(social.twitter,	ref plausibility);
            check_social(social.skype,		ref plausibility);
            check_social(social.livejournal,ref plausibility);
            check_social(social.instagram,	ref plausibility);
            check_social(social.facebook,	ref plausibility);
            
            if (city.city_id == 1463)
                Plausibility += 4;
            else if (city.city_id == 1)
                Plausibility++;
            else
                Plausibility -= 3;
            
            if (contacts.mobile_phone != null)
                Plausibility++;
            if (contacts.home_phone != null)
                Plausibility++;

            if (bdate.Year != DateTime.MinValue.Year)
                Plausibility++;
            
            if (Plausibility > 40)
                Plausibility = 40;
            if (Plausibility < 0)
                Plausibility = 0;
            return Plausibility;
        }

        public override string ToString()
        {
            string user_info = vk_id + " " + first_name + " " + last_name + " " + (sex ? "Муж" : "Жен") + " " + bdate.ToShortDateString() + " " + city.city_title + " " + arrived_from + "\n"
                + (contacts.mobile_phone != null ? " Моб.телефон:" + contacts.mobile_phone : "") + (contacts.home_phone != null ? " Дом.телефон:" + contacts.home_phone : "")
                + (social.instagram != null ? " instagram:" + social.instagram : "")
                + (social.skype != null ? " skype:" + social.skype : "")
                + (social.facebook != null ? " facebook:" + social.facebook : "")
                + (social.livejournal != null ? " livejournal:" + social.livejournal : "")
                + (social.twitter != null ? " twitter:" + social.twitter : "")
                + "\n" + _id + "\nplausibility " + Plausibility + "\n";

            foreach (University university in universities)
            {
                user_info += university.university_name + " " 
                    + university.faculty_name + " " 
                    + (university.graduation_year != 0 ? university.graduation_year.ToString() : "") + "\n";
            }   
            
            return user_info;
        }

        // TODO Пока простая проверка
        public static bool operator ==(Human user1, Human user2)
        {
            return user1.vk_id == user2.vk_id
                && user1.first_name == user2.first_name
                && user1.last_name == user2.last_name;
        }

        public static bool operator !=(Human user1, Human user2)
        {
            return user1.vk_id != user2.vk_id
               || user1.first_name != user2.first_name
               || user1.last_name != user2.last_name;
        }

    }
}
