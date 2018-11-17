using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;

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

    public class Human
    {
        public ObjectId _id { get; set; }
        public int id; // TODO Если будет несколько сетей, то можно запутаться в ID
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
        public HashSet<string> emails { get { return contacts.emails; } set { contacts.emails = value; } }
        public string email { get { return contacts.emails != null ? contacts.emails.ElementAt(0) : ""; } }

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
        public string vk { get { return "https://vk.com/" + domain; } } // TODO Потом вк не всегда будет 

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

        public HashSet<string> sites {get; set; }

        public HashSet<Human> friends{get; set;}
        // Дефолтный конструктор нужен для бд
        public Human()
        {
            id = 0;
            first_name = null;
            last_name = null;
            sex = false;
            domain = null;
            bdate = new DateTime();

            city = new City();


            contacts = new Contacts();

            social = new Social();

            photo_100 = null;
            arrived_from = null;

            plausibility = 0;
        }

        public Human(JToken user_data)
        {
            id = (int) user_data["id"];
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

        //если внести graduation_year в Human, то надо убрать передачу аргумента
        public int CalcPlausibility()
        {
            plausibility = 0;

            if (AnalyzeData.CheckAge(bdate,	DateTime.Parse(graduation_year)))
            	plausibility++;

            if (city.city_title.ToLower().Equals("зеленоград"))
                plausibility += 2;
            else if (city.city_title.ToLower().Equals("москва"))
                plausibility++;

            //TODO пусть дефолтным будет миэт, тогда, думаю, graduation_year нужно будет из него вынести
            if (universities.Contains(default(University)))
                plausibility += 20;
            else
                plausibility -= 2;
            
            if(AnalyzeData.CheckFriends(friends))
            	plausibility += 4;
            
            if(AnalyzeData.CheckArrival(arrived_from))
            	plausibility += 3;

            return plausibility;
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
                + "\n" + _id + "\n";

            foreach (University university in universities)
            {
                user_info += university.university_name + " " 
                    + university.faculty_name + " " 
                    + (university.graduation_year != 0 ? university.graduation_year.ToString() : "") + "\n";
            }   
            
            user_info += "\n-----\n";
            foreach(string email in emails){
            	user_info += "\n" + email;
            }
            
            user_info += "\n~~~~~\n";
            foreach(string site in sites){
            	user_info += "\n" + site;
            }
            
            return user_info;
        }
        // TODO Пока простая проверка
        //public bool Equals(Human user)
        //{
        //    //Human user = (Human)obj;
        //    return id == user.id && first_name == user.first_name && last_name == user.last_name;
        //}

        public static bool operator ==(Human user1, Human user2)
        {
            return user1.id == user2.id
                && user1.first_name == user2.first_name
                && user1.last_name == user2.last_name;
        }

        public static bool operator !=(Human user1, Human user2)
        {
            return user1.id != user2.id
               || user1.first_name != user2.first_name
               || user1.last_name != user2.last_name;
        }

    }
}
