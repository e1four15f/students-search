using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        private ObservableCollection<Human> response_users;
        private ObservableCollection<Human> selected_users;

        public Human SelectedHuman { get; set; }
        public Human SelectedListItem { get; set; }

        private ListBox response_list_box;
        private ListBox selected_users_list_box;
        private Label response_info;

        private bool saved;
        
        public Search(string current_file = "Новый список")
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            response_list_box = (ListBox) FindName("ResponseListBox");
            selected_users_list_box = (ListBox) FindName("SelectedUsersListBox");
            selected_users = new ObservableCollection<Human>();

            response_info = (Label) FindName("ResponseInfo");
            saved = true;
            this.Title = current_file;
            /*
            LoadDB("data/full.json");
            UpdateResponse(db_users);
            */
            //MainWindow.db_users = new DBCreator().LoadDB("data/users_data_27_10_2018.json");
            /* Для теста 
            List<Human> random_users = new List<Human>();
            Random rnd = new Random();
            for (int i = 0; i < 50; i++)
            {
                int r = rnd.Next(db_users.Count);
                random_users.Add(db_users[r]);
            }
            UpdateResponse(random_users);*/
        }

        // TODO Разобраться с ошибкой ArgumentOutOfRange
        /* Обновляет окно результатов поиска пользователей */
        private void UpdateResponse(List<Human> users_from_db)
        {
            response_users = new ObservableCollection<Human>(users_from_db); //new List<Human>();
            // TODO Выбрать оптимальное значение 
            if (response_users.Count < 1000)
            {
                response_list_box.SetValue(ScrollViewer.CanContentScrollProperty, false);
            }
            else
            {
                response_list_box.SetValue(ScrollViewer.CanContentScrollProperty, true);
            }

            response_info.Content = "Найденно " + response_users.Count + " пользователей";
            response_list_box.ItemsSource = response_users;
        }

        /* Кнопки */
        /* Открывает окно создания запроса к бд */
        private void ButtonMakeRequest(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сделать запрос");
            MakeRequest make_request = new MakeRequest();
            make_request.ShowDialog();

            Console.WriteLine(make_request.FirstName.Text + " : " + make_request.LastName.Text);
            /* Временное решение пока нет бд*/
            if (MainWindow.db.Users() != null)
            { 
                List<Human> criterion_users = new List<Human>();
                foreach (Human user in MainWindow.db.Users())
                {
                    bool criterion = false;
                    if (make_request.FirstName.Text == "*")
                    {
                        criterion = true;
                    }
                    else
                    { 
                        criterion = make_request.FirstName.Text != "" ? user.first_name == make_request.FirstName.Text : true;
                        criterion &= make_request.LastName.Text != "" ? user.last_name == make_request.LastName.Text : true;
                    }
                    if (criterion)
                    {
                        criterion_users.Add(user);
                    }
                }
                Console.WriteLine(criterion_users.Count);
                
                if (criterion_users.Count != MainWindow.db.Users().Count || make_request.FirstName.Text == "*")
                {
                    UpdateResponse(criterion_users);
                }
            }
        }

        /* Добавляет выбранного в панели результатов поиска пользователя в список */
        private void ButtonAddToList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Добавить в список");
            // TODO Это плохое решение задачи
            try
            {
                // TODO При одноклассниках стоит использовать другую проверку
                if (selected_users.Where(x => x.id == SelectedHuman.id).ToList().Count() == 0)
                {
                    selected_users.Add(SelectedHuman);
                    selected_users_list_box.ItemsSource = selected_users;
                    if (saved)
                    {
                        this.Title += "*";
                    }
                    saved = false;
                }
                else
                {
                    Console.WriteLine("Данный пользователь уже находится в списке");
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /* Удаляет выбранного в списке пользователя */
        private void ButtonRemoveFromList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Удалить из списка");
            if (selected_users.Count > 0)
            {
                // TODO Это плохое решение задачи
                try
                {
                    // TODO При одноклассниках стоит использовать другую проверку
                    if (selected_users.Where(x => x.id == SelectedListItem.id).ToList().Count() == 1)
                    {
                        selected_users.Remove(SelectedListItem);
                        selected_users_list_box.ItemsSource = selected_users;
                        if (saved)
                        {
                            this.Title += "*";
                        }
                        saved = false;
                    }
                    else
                    {
                        Console.WriteLine("Что-то пошло не так");
                    }
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /* Открывает подробную информацио о выбранном в панели результатов поиска пользователя */
        private void ButtonMoreInfo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Подробная информация");
            try
            { 
                MessageBox.Show(SelectedHuman.ToString());
            } 
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        /* События */
        /* Исполняется при закрытии окна. Проверяет сохранён ли список при выходе */
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine(this.ToString() + ": Закрытие окна");
            if (!saved)
            {
                MessageBoxResult dialog = MessageBox.Show("Сохранить изменения в " + this.Title.Remove(this.Title.Length - 1) + "?",
                    "Exit", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (dialog == MessageBoxResult.Yes)
                {
                    if (MenuController.SaveList(this, saved))
                    {
                        Console.WriteLine(this.ToString() + ": Выход с сохранением списка");
                        saved = true;
                    }
                    else
                    { 
                        return;
                    }
                }
                else if (dialog == MessageBoxResult.No)
                {
                    Console.WriteLine(this.ToString() + ": Выход без сохранения списка");
                    saved = true;
                }
                else if (dialog == MessageBoxResult.Cancel)
                {
                    Console.WriteLine(this.ToString() + ": Отмена");
                    return;
                }
            }
            e.Cancel = false;
            App.Current.MainWindow.Show();
            base.OnClosing(e);
        }
        
        /* Обработчик события для перехода на url */
        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /* Меню */
        /* Создаёт новую панель поиска */
        private void MenuNewList(object sender, RoutedEventArgs e)
        {
            MenuController.NewList(this, saved);
        }

        /* Загружает список */
        private void MenuLoadList(object sender, RoutedEventArgs e)
        {
            MenuController.LoadList(this, saved);
        }

        /* Сохраняет список */
        private void MenuSaveList(object sender, RoutedEventArgs e)
        {
            saved = MenuController.SaveList(this, saved);
        }

        /* Загружает БД */
        private void MenuLoadDB(object sender, RoutedEventArgs e)
        {
            MenuController.LoadDB(this);
        }

        /* Информация о БД */
        private void MenuDBInfo(object sender, RoutedEventArgs e)
        {
            MenuController.DBInfo(this);
        }

        /* Выход */
        private void MenuExit(object sender, RoutedEventArgs e)
        {
            MenuController.Exit(this);
        }
    }
}
