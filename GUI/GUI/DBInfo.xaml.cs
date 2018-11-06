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

            db = new DatabaseAPI(DatabaseAPI.DEFAULT_DB);
            dbFile = db.getDBFileInfo();

            db_date_of_creation = dbFile.CreationTime;
            db_users_count = db.getUserCount();
            db_size_in_mb = dbFile.Length / 1024 / 1024;

            Label date_of_creation = (Label) this.FindName("DateOfCreation");
            Label users_count = (Label) this.FindName("UsersCount");
            Label size_in_mb = (Label) this.FindName("SizeInMb");

            date_of_creation.Content = "Дата создания БД: " + db_date_of_creation;
            users_count.Content = "Количество записей в БД: " + db_users_count + " пользователей";
            size_in_mb.Content = "Размер БД: " + db_size_in_mb + " мб";
        }
    }
}
