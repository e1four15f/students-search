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
        private DatabaseAPI db;
        private FileInfo dbFile;

        private DateTime db_date_of_creation;
        private long db_users_count;
        private long db_size_in_mb;

        public AboutDB()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            // TODO Нужна ли проверка if (db == null || db.Users() == null) для вывода, что нет бд?
            db = new DatabaseAPI(DatabaseAPI.DEFAULT_DB);
            dbFile = db.getDBFileInfo();

            db_date_of_creation = dbFile.CreationTime;
            db_users_count = db.getUserCount();
            db_size_in_mb = dbFile.Length / 1024 / 1024;

            DateOfCreation.Content = "Дата создания БД: " + db_date_of_creation;
            UsersCount.Content = "Количество записей в БД: " + db_users_count + " пользователей";
            SizeInMb.Content = "Размер БД: " + db_size_in_mb + " мб";
            /*
             if (db == null || db.Users() == null)
            {
                Label date_of_creation = (Label) FindName("DateOfCreation");

                date_of_creation.Content = "База данных не загружена!";
                date_of_creation.FontSize = 20;
                date_of_creation.HorizontalAlignment = HorizontalAlignment.Center;
                date_of_creation.Foreground = new SolidColorBrush(Colors.Red);
            }
             */
        }
    }
}
