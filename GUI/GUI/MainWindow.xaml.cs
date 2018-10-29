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
        // TODO Возможно стоит сделать отдельные методы, чтобы код не повторялся 
        /* Кнопки */
        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Поиск");
            Search search = new Search();
            this.Hide();
            search.Show();
        }

        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сформировать БД");
            CreateDB create_db = new CreateDB();
            create_db.ShowDialog();
        }

        private void ButtonAboutDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Информация о БД");
            AboutDB about_db = new AboutDB();
            about_db.ShowDialog();
        }

        private void ButtonSettings(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Настройки");
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void ButtonExit(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Выход");
            this.Close();
        }

        /* Меню */
        private void MenuNewList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Новый список");
            Search search = new Search();
            this.Hide();
            search.Show();
        }
            
        private void MenuLoadList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": загрузить список");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            Console.WriteLine(this.ToString() + ": Загружен список: " + open_file_dialog.FileName);
        }

        private void MenuLoadDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Загрузить БД");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            Console.WriteLine(this.ToString() + ": Загружена БД: " + open_file_dialog.FileName);
        }

        private void MenuExit(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Выход");
            this.Close();
        }

        private void MenuDBInfo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Информация о БД");
            AboutDB about_db = new AboutDB();
            about_db.ShowDialog();
        }

        private void MenuSettings(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Настройки");
            Settings settings = new Settings();
            settings.ShowDialog();
        }
    }
}
