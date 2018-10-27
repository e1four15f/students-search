using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyzeAndProcess;

namespace HumanData
{
    // TODO Нужно ли в структурах хранить и id, и название?
    // я думаю, что нужно, для вывода информации
    struct City
    {
        public int city_id;
        public string city_title; 
    }

    struct University
    {
        public int university_id;
        public string university_name;
        public int faculty_id;
        public string faculty_name;
        
        //TODO может вынести из класса?
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
        public string mobile_phone;
        public string home_phone;

        public string skype;
        public string facebook;
        public string instagram;

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
             //city; City
            //mobile_phone; string
            //home_phone; string
            /*
            skype; string
            facebook; string
            instagram; string

            photo_100; string*/
        }

        //если внести graduation_year в Human, то надо убрать передачу аргумента
        public int CalcPlausibility(DateTime graduation)
        {
        	plausibility = 0;
			
        	if(AnalyzeData.IsPresent(last_name, first_name, bdate, graduation,ref plausibility));
			
        	if(city.city_title.ToLower().Equals("зеленоград"))
        		plausibility += 2;
			else if(city.city_title.ToLower().Equals("москва"))
				plausibility++;
			
			//TODO пусть дефолтным будет миэт, тогда, думаю, graduation_year нужно будет из него вынести
			if(universities.Contains(default(University)))
				plausibility += 20;
			else 
				plausibility -= 2;
			
            return plausibility;
        }

        public override string ToString()
        {
            string user_info = domain + " " + first_name + " " + last_name + " " + (sex ? "Муж" : "Жен") + " " + bdate.ToShortDateString()
                + " " + city.city_title;

            return user_info;
        }
    }

 
}




















