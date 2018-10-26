using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    // TODO Нужно ли в структурах хранить и id, и название?
    // TODO Разобраться как хранить переменные 
    struct City
    {
        public int city_id;
        public string city_title; 
    }

    struct Contacts
    {
        public string mobile_phone;
        public string home_phone;
        public string email;
    }

    struct Social
    {
        public string skype;
        public string facebook;
        public string instagram;
    }

    struct University
    {
        public int university_id;
        public string university_name;
        public int faculty_id;
        public string faculty_name;
        public int graduation_year;
    }

    class Human
    {
        public int id;
        public string first_name;
        public string last_name;
        public bool sex;
        public string domain;
        public DateTime bdate;
        public City city;
        public Contacts contacts;
        public Social social;
        public string photo_100;
        public List<University> universities;

        public int plausibility;

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
                        bdate = (splited_date.Length == 3) ? new DateTime(splited_date[2], splited_date[1], splited_date[0]) : new DateTime(2000, splited_date[1], splited_date[0]);
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

            contacts.mobile_phone = user_data["mobile_phone"] != null ? user_data["mobile_phone"].ToString() : null;
            contacts.home_phone = user_data["home_phone"] != null ? user_data["home_phone"].ToString() : null;
            // TODO Проверить в данных наличие других сетей
            social.instagram = user_data["instagram"] != null ? user_data["instagram"].ToString() : null;
            social.skype = user_data["skype"] != null ? user_data["skype"].ToString() : null;
            social.facebook = user_data["facebook"] != null ? user_data["facebook"].ToString() : null;

            photo_100 = user_data["photo_100"].ToString();

            universities = new List<University>();
            if (user_data["occupation"] != null && user_data["occupation"]["type"].ToString() == "university")
            {
                University occupation = new University();
                occupation.university_id = (int) user_data["occupation"]["id"];
                occupation.university_name = user_data["occupation"]["name"].ToString();
                universities.Add(occupation);
            }
            
            if (user_data["universities"] != null)
            {
                foreach (JToken university_data in user_data["universities"])
                {
                    if (university_data.Type != JTokenType.Null)
                    {
                        University university = new University();
                        university.university_id = (int) university_data["id"];
                        university.university_name = university_data["name"].ToString();
                        if (university_data["faculty"] != null)
                        {
                            university.faculty_id = (int) university_data["faculty"];
                            university.faculty_name = university_data["faculty_name"].ToString();
                        }
                        university.graduation_year = university_data["graduation"] != null ? int.Parse(university_data["graduation"].ToString()) : 0;
                        universities.Add(university);
                    }
                }
            }
             
        }

        // TODO quick math
        public int CalcPlausibility()
        {
            return 100;
        }

        public override string ToString()
        {
            string user_info = domain + " " + first_name + " " + last_name + " " + (sex ? "Муж" : "Жен") + " " + bdate.ToShortDateString() + " " + city.city_title + "\n"
                + (contacts.mobile_phone != null ? " Моб.телефон:" + contacts.mobile_phone : "") + (contacts.home_phone != null ? " Дом.телефон:" + contacts.home_phone : "")
                + (social.instagram != null ? " instagram:" + social.instagram : "")
                + (social.skype != null ? " skype:" + social.skype : "")
                + (social.facebook != null ? " facebook:" + social.facebook : "") 
                + "\n";

            return user_info;
        }
    }

 
}




















