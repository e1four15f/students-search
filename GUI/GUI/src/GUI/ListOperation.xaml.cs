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
using System.Diagnostics;

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
            
            Stopwatch timer = new Stopwatch();
            bool timer_switch = true;
            
            if (timer_switch){
    			timer.Start();
    			ProcessData.GenerateAddr(users[0]);
    			ProcessData.GetEmails("example@example.com");
            	timer.Stop(); 
            	timer_switch = false;
            	MessageBox.Show("Примерное время окончания процедуры: " + DateTime.Now.AddMilliseconds(timer.ElapsedMilliseconds*users.Count).ToShortTimeString());
            }
            
            new System.Threading.Thread(() =>{
		        
		        foreach (Human user in users){
		            result.Add(user);
		            Parallel.ForEach(ProcessData.GenerateAddr(user), email =>
		            {		                
		                emails.TryAdd(user, ProcessData.GetEmails(email));   
		            });
		            
		        }
		        foreach (Human user in result){
		            HashSet<string> temp = new HashSet<string>();
		            if (emails.TryGetValue(user, out temp)) { 
		            	
		            	
		            	if(user.contacts.emails != null)
		                    user.contacts.emails.UnionWith(new HashSet<string>(temp));
		            	else
		                    user.contacts.emails = new HashSet<string>(temp);
		            		
		            	temp.Clear();
		            }
		        }
				MessageBox.Show("Email'ы собраны");
            }).Start();
			
        }

        private void ButtonMostLikelySites(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<Human, HashSet<string>> sites = new ConcurrentDictionary<Human, HashSet<string>>();
            HashSet<Human> result = new HashSet<Human>();
            List<Action> get_sites = new List<Action>();
            
            Stopwatch timer = new Stopwatch();
            bool timer_switch = true;
            
            if (timer_switch){
    			timer.Start();
    			ProcessData.MostLikelySites(users[0]);
            	timer.Stop(); 
            	timer_switch = false;
            	MessageBox.Show("Примерное время окончания процедуры: " + DateTime.Now.AddMilliseconds(timer.ElapsedMilliseconds*users.Count).ToShortTimeString());
            }
            new System.Threading.Thread(() =>{
                                        	
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
		
		                if (user.contacts.sites != null)
		                    user.contacts.sites.UnionWith(new HashSet<string>(temp));
		            	else
		                    user.contacts.sites = new HashSet<string>(temp);
		                temp.Clear();
		            }
		        }
				MessageBox.Show("Аккаунты на популярных сайтах собраны");
            }).Start();
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
           		throw new Exception("В ресурсах программы нет user-agent, сообщите разработчикам");
            
            
            Stopwatch timer = new Stopwatch();
            bool timer_switch = true;
        
            if (timer_switch){
    			timer.Start();
    			ProcessData.SearchInNet(users[0].domain, user_agents);
            	timer.Stop(); 
            	timer_switch = false;
            	MessageBox.Show("Примерное время окончания процедуры: " + DateTime.Now.AddMilliseconds(timer.ElapsedMilliseconds*users.Count).ToShortTimeString());
            }
            
            new System.Threading.Thread(() =>{
                                        	
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
	
	                    if (user.contacts.sites != null)
	                        user.contacts.sites.UnionWith(new HashSet<string>(temp));
	                	else
	                        user.contacts.sites = new HashSet<string>(temp);
	                    temp.Clear();
	                }
	            }
				MessageBox.Show("Поисковая выдача собрана");
            }).Start();
            return;
        }
    }
}
