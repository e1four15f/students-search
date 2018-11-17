using System.IO;
using System.Windows;
using System.Windows.Media;

namespace GUI
{
    /// <summary>
    /// Interaction logic for AboutDB.xaml
    /// </summary>
    public partial class AlertDB : Window
    {
        private FileInfo dbFile;

        public AlertDB()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            DateOfCreation.Content = "База данных не загружена!\nЧтобы загрузить базу данных нажмите\n\"Файл -> Загрузить БД\"";
            DateOfCreation.FontSize = 14;
            DateOfCreation.HorizontalAlignment = HorizontalAlignment.Center;
            DateOfCreation.Foreground = new SolidColorBrush(Colors.Red);
        }

    }
}
