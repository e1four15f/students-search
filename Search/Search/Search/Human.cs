using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    // TODO Нужно ли в структурах хранить и id, и название?
    struct City
    {
        int city_id;
        string city_title; 
    }

    struct University
    {
        int university_id;
        string university_name;
        int faculty_id;
        string faculty_name;
        int graduation_year;
    }

    class Human
    {
        int id;
        string first_name;
        string last_name;
        string domain;
        DateTime bdate;
        City city;
        string mobile_phone;
        string home_phone;

        string skype;
        string facebook;
        string instagram;

        string photo_100;
        University[] universities;

        public Human(JToken user_data)
        {
            //id = (int) user_data["id"];
        }
    }

 
}




















