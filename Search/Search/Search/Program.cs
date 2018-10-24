using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Search
{
    class Program
    {
        static void Main(string[] args)
        {
            VkApiUtils.Auth();

            //Console.WriteLine(VkApi.UsersGet(LoadFile("search_users_ids").GetRange(0, 400), VkApiUtils.fields)); 
            //Console.WriteLine(String.Join(",", VkApi.GroupsSearch("миэт", 3).ToArray()));
            //Console.WriteLine(String.Join(",", VkApi.GroupsGetMembers("mietoverhear", 100).ToArray())); 
            //Console.WriteLine(String.Join(",", VkApi.GroupsGetById(new List<int>() { 123, 312, 412521, 521 }).ToArray()));
            //Console.WriteLine(String.Join(",", VkApi.UsersSearch("Дима", 10).ToArray())); 
            //Console.WriteLine(VkApiAdvanced.HiddenMietGroupCheck("mp_15a_2016")); 
            //Console.WriteLine(String.Join(",", VkApiAdvanced.GetHiddenGroups("миэт").ToArray()));
            //JArray users_data = VkApiMulti.UsersGet(LoadFile("search_users_ids"), VkApiUtils.fields); 

            new DBCreator().Create();

            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
