using Microsoft.Win32;
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

using WebApi;
using DB;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool auth;
        public static List<Human> db_users;

        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            auth = false;
            db_users = new List<Human>();
        }

        /* Кнопки */
        /* Формирует бд */
        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сформировать БД");
            if (!auth)
            {
                auth = VkApiUtils.Auth();
            }

            if (auth)
            {
                new CreateDB().ShowDialog();
            }
            else 
            {
                new Login().ShowDialog(); 
            }
        }

        /* Меню */
        /* Загружает список */
        private void MenuLoadList(object sender, RoutedEventArgs e)
        {
            MenuController.LoadList(this);
        }

        /* Загружает БД */
        private void MenuLoadDB(object sender, RoutedEventArgs e)
        {
            MenuController.LoadDB(this);
        }

        /* Общие обработчики */
        /* Создаёт новую панель поиска */
        private void NewList(object sender, RoutedEventArgs e)
        {
            MenuController.NewList(this);
        }

        /* Информация о БД */
        private void DBInfo(object sender, RoutedEventArgs e)
        {
            MenuController.DBInfo(this);
        }

        /* Настройки */
        private void Settings(object sender, RoutedEventArgs e)
        {
            MenuController.Settings(this);
        }

        /* Выход */
        private void Exit(object sender, RoutedEventArgs e)
        {
            MenuController.Exit(this);
        }
    }
}
