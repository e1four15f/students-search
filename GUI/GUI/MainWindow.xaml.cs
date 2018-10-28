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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void ButtonAboutDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Информация о БД");
            AboutDB about_db = new AboutDB();
            about_db.ShowDialog();
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Поиск");
            Search search = new Search();
            search.ShowDialog();
        }

        private void ButtonExit(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Выход");
            this.Close();
        }

        private void ButtonSettings(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Настройки");
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Сформировать БД");
            CreateDB create_db = new CreateDB();
            create_db.ShowDialog();
        }
    }
}
