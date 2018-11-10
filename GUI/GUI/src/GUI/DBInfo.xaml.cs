using System;
using System.Collections.Generic;
using System.IO;
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
        private FileInfo dbFile;

        private DateTime db_date_of_creation;
        private long db_users_count;
        private long db_size_in_mb;

        public AboutDB()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            if (MainWindow.db == null || MainWindow.db_users.Count == 0) 
            {
                DateOfCreation.Content = "База данных не загружена!";
                DateOfCreation.FontSize = 20;
                DateOfCreation.HorizontalAlignment = HorizontalAlignment.Center;
                DateOfCreation.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            { 
                dbFile = MainWindow.db.getDBFileInfo();

                db_date_of_creation = dbFile.CreationTime;
                db_users_count = MainWindow.db_users.Count;
                db_size_in_mb = dbFile.Length / 1024 / 1024;

                DateOfCreation.Content = "Дата создания БД: " + db_date_of_creation;
                UsersCount.Content = "Количество записей в БД: " + db_users_count + " пользователей";
                SizeInMb.Content = "Размер БД: " + db_size_in_mb + " мб";
            }
        }
        
    }
}
