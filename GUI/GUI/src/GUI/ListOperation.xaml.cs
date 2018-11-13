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
            foreach (Human user in users)
            {
                List<string> emails = new List<string>();
                foreach (string email in ProcessData.GenerateAddr(user))
                {
                    Console.WriteLine(user.first_name + ": " + email);
                    //emails = emails.Concat(ProcessData.GetEmails(email)).ToList();

                    //foreach (string s in emails)
                     //   Console.WriteLine(user.first_name + ": " + s);
                }
                user.emails = new List<string>(emails);
                emails.Clear();
            }
        }
    }
}
