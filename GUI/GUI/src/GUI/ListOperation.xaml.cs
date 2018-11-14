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
            ConcurrentDictionary<Human, List<string>> emails = new ConcurrentDictionary<Human, List<string>>();
            HashSet<Human> result = new HashSet<Human>();
            foreach (Human user in users)
            {
                result.Add(user);
                Parallel.ForEach(ProcessData.GenerateAddr(user), email =>
                {
                    Console.WriteLine(user.first_name + ": " + email);
                    
                    emails.TryAdd(user, ProcessData.GetEmails(email));//.ToList();
                    
                });
                /*
                foreach (KeyValuePair<Human, List<string>> kvp in emails)
                    foreach (string s in kvp.Value)
                        Console.WriteLine("ALL " + user.first_name + ": " + s);*/
                
            }
            foreach (Human user in result)
            {
                List<string> temp = new List<string>();
                if (emails.TryGetValue(user, out temp)) { 
                    user.emails = new List<string>(temp);

                foreach (string s in temp)
                    Console.WriteLine("ALL " + user.first_name + ": " + s);

                temp.Clear();
                }
            }
        }

        private void ButtonMostLikelySites(object sender, RoutedEventArgs e)
        {
            ConcurrentDictionary<Human, List<string>> sites = new ConcurrentDictionary<Human, List<string>>();
            HashSet<Human> result = new HashSet<Human>();
            foreach (Human user in users)
            {
                result.Add(user);
                Parallel.ForEach(ProcessData.MostLikelySites(user), site =>
                {
                    Console.WriteLine(user.first_name + ": " + site);

                    sites.TryAdd(user, ProcessData.GetEmails(site));//.ToList();

                });
                /*
                foreach (KeyValuePair<Human, List<string>> kvp in emails)
                    foreach (string s in kvp.Value)
                        Console.WriteLine("ALL " + user.first_name + ": " + s);*/

            }
            foreach (Human user in result)
            {
                List<string> temp = new List<string>();
                if (sites.TryGetValue(user, out temp))
                {
                    user.emails = new List<string>(temp);

                    foreach (string s in temp)
                        Console.WriteLine("ALL " + user.first_name + ": " + s);

                    temp.Clear();
                }
            }
        }
    }
}
