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

namespace GUI
{
    /// <summary>
    /// Interaction logic for AboutDB.xaml
    /// </summary>
    public partial class AboutDB : Window
    {
        private DateTime db_date_of_creation = DateTime.Now;
        private int db_users_count = new Random().Next(10000, 100000);
        private int db_size_in_mb = new Random().Next(1, 1000);

        public AboutDB(int db_users_count = 0)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            this.db_users_count = db_users_count;

            Label date_of_creation = (Label) this.FindName("DateOfCreation");
            Label users_count = (Label) this.FindName("UsersCount");
            Label size_in_mb = (Label) this.FindName("SizeInMb");

            date_of_creation.Content = "Дата создания БД: " + db_date_of_creation;
            users_count.Content = "Количество записей в БД: " + db_users_count + " пользователей";
            size_in_mb.Content = "Размер БД: " + db_size_in_mb + " мб";
        }
    }
}
