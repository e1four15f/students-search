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

namespace GUI
{
    /// <summary>
    /// Interaction logic for AboutDB.xaml
    /// </summary>
    public partial class AboutDB : Window
    {
        public AboutDB(DBConteiner db = null)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            if (db == null || db.Users() == null)
            {
                Label date_of_creation = (Label) FindName("DateOfCreation");

                date_of_creation.Content = "База данных не загружена!";
                date_of_creation.FontSize = 20;
                date_of_creation.HorizontalAlignment = HorizontalAlignment.Center;
                date_of_creation.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            { 
                Label date_of_creation = (Label) FindName("DateOfCreation");
                Label users_count = (Label) FindName("UsersCount");
                Label size_in_mb = (Label) FindName("SizeInMb");

                date_of_creation.Content = "Дата создания БД: " + db.Info().LastWriteTime;
                users_count.Content = "Количество записей в БД: " + db.Users().Count + " пользователей";
                size_in_mb.Content = "Размер БД: " + Math.Round(db.Info().Length / Math.Pow(2, 20), 2) + " мб";
            }
        }
    }
}
