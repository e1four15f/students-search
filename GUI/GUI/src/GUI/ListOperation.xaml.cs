using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Resources;
using System.IO;

using DB;
using Utils;

namespace GUI
{
    /// <summary>
    /// Interaction logic for ListOperation.xaml
    /// </summary>
    public partial class ListOperation : Window
    {
        private List<Human> users;

        public ListOperation(List<Human> users)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            this.users = users;
        }

        private void ButtonFindEmails(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<Human, HashSet<string>> emails = new ConcurrentDictionary<Human, HashSet<string>>();
            HashSet<Human> result = new HashSet<Human>();
            foreach (Human user in users)
            {
                result.Add(user);
                Parallel.ForEach(ProcessData.GenerateAddr(user), email =>
                {
                    Console.WriteLine(user.first_name + ": " + email);
                    
                    emails.TryAdd(user, ProcessData.GetEmails(email));//.ToList();
                    
                });
                
            }
            foreach (Human user in result)
            {
                HashSet<string> temp = new HashSet<string>();
                if (emails.TryGetValue(user, out temp)) { 
                	
                	user.emails.UnionWith(new HashSet<string>(temp));
                	temp.Clear();
                }
            }
            
			Console.Beep();
        }

        private void ButtonMostLikelySites(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<Human, HashSet<string>> sites = new ConcurrentDictionary<Human, HashSet<string>>();
            HashSet<Human> result = new HashSet<Human>();
            List<Action> get_sites = new List<Action>();
            
            foreach (Human user in users)
            {
                result.Add(user);
                get_sites.Add(() => sites.TryAdd(user, ProcessData.MostLikelySites(user)));
            }
            Parallel.Invoke(get_sites.ToArray());
            foreach (Human user in result)
            {
                HashSet<string> temp = new HashSet<string>();
                if (sites.TryGetValue(user, out temp))
                {
                	if(temp.Count == 0)
                		continue;
                	
                	user.sites.UnionWith(new HashSet<string>(temp));
                    temp.Clear();
                }
            }
            
			Console.Beep();
            return;
        }
        
        private void ButtonGoogleIt(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<Human, HashSet<string>> sites = new ConcurrentDictionary<Human, HashSet<string>>();
            HashSet<Human> result = new HashSet<Human>();
            List<Action> get_sites = new List<Action>();
            List<string> user_agents;
            
            string serialized_agents = Encoding.Default.GetString(Properties.Resources.user_agents);

            if(serialized_agents != null)
          		user_agents =  JsonConvert.DeserializeObject<List<string>>(serialized_agents);
            else
           		throw new Exception("где агенты, джонни???");
            
            foreach (Human user in users)
            {
                result.Add(user);
                if(user.domain != null)
                	get_sites.Add(() => sites.TryAdd(user, ProcessData.SearchInNet(user.domain, user_agents)));
                
                if(user.instagram != null)
                	get_sites.Add(() => sites.TryAdd(user, ProcessData.SearchInNet(user.instagram, user_agents)));
                
                if(user.facebook != null)
               		get_sites.Add(() => sites.TryAdd(user, ProcessData.SearchInNet(user.facebook, user_agents)));
                
                if(user.twitter != null)
                	get_sites.Add(() => sites.TryAdd(user, ProcessData.SearchInNet(user.twitter, user_agents)));
                
                if(user.skype != null)
                	get_sites.Add(() => sites.TryAdd(user, ProcessData.SearchInNet(user.skype, user_agents)));
            }
            Parallel.Invoke(get_sites.ToArray());
            foreach (Human user in result)
            {
                HashSet<string> temp = new HashSet<string>();
                if (sites.TryGetValue(user, out temp))
                {
                	if(temp.Count == 0)
                		continue;
                	user.sites.UnionWith(new HashSet<string>(temp));
                    temp.Clear();
                }
            }
            
			Console.Beep();
            return;
        }
        
    }
}
