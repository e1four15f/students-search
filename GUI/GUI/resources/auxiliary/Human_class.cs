using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace ChangeMe_Namespace
{
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

    public class Human
    {
        public ObjectId _id { get; set; }
        public int vk_id { get; set; }
        public string ok_id { get; set; } 
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool sex { get; set; }

        public string Sex { get { return sex ? "Мужчина" : "Женщина"; } }
        public string domain { get; set; }
        public DateTime bdate { get; set; }
        public string BDate { get { return bdate != new DateTime() ? bdate.Year > DateTime.MinValue.Year ? bdate.ToShortDateString() : bdate.Day + "." + bdate.Month : ""; } }
        
		public City city { get; set; }
        public string city_title { get { return city.city_title; } }
		
        public Contacts contacts { get; set; }
        public string mobile_phone { get { return contacts.mobile_phone != null ? "Моб. " + contacts.mobile_phone : ""; } }
        public string home_phone { get { return contacts.home_phone != null ? "Дом. " + contacts.home_phone : ""; } }
        public string email { get { return contacts.emails != null ? contacts.emails.ElementAt(0) : ""; } }
		
        public Social social { get; set; } // TODO // TODO Odnoklassniki
        public string skype { get { return social.skype != null ? "skype:" + social.skype + "?call" : null; } }
        public string facebook { get { return social.facebook != null ? "https://www.facebook.com/" + social.facebook : null; } }
        public string instagram { get { return social.instagram != null ? "https://www.instagram.com/" + social.instagram : null; } }
        public string livejournal { get { return social.livejournal != null ? "https://" + social.livejournal + ".livejournal.com/" : null; } }
        public string twitter { get { return social.twitter != null ? "https://twitter.com/" + social.twitter : null; } }
        public string vk { get { return domain != null ? "https://vk.com/" + domain : null; } } 
        public string ok { get { return ok_id != null ? "https://ok.ru/profile/" + ok_id : null; } } 

        public List<University> universities { get; set; }
        public string university_name { get { return universities.Count > 0 ? universities[0].university_name : ""; } }
        public string faculty_name { get { return universities.Count > 0 ? universities[0].faculty_name : ""; } }
        public string chair_name { get { return universities.Count > 0 ? universities[0].chair_name : ""; } }
        public string graduation_year { get { return (universities.Count > 0 && universities[0].graduation_year > 0) ? "Год окончания " + universities[0].graduation_year : ""; } }

        public string photo_100 { get; set; }
        public string arrived_from { get; set; }
        
        public int plausibility;
        public int Plausibility { get { return plausibility; } set { plausibility = value; } }
}
