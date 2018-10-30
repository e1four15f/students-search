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

namespace GUI
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        private List<Human> users;
        private ObservableCollection<Human> selected_users;
        public Human SelectedHuman { get; set; }
        public Human SelectedListItem { get; set; }

        private ListBox response_list_box = new ListBox();
        private ListBox selected_users_list_box = new ListBox();
        
        public Search()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            response_list_box = (ListBox) FindName("ResponseListBox");
            selected_users_list_box = (ListBox) FindName("SelectedUsersListBox");
            selected_users = new ObservableCollection<Human>();
            UpdateResponse("data/full.json");
        }

        // TODO Разобраться с ошибкой ArgumentOutOfRange
        private void UpdateResponse(string filename)
        {
            users = new List<Human>();
            foreach (JToken user_data in LoadFileJson(filename))
            {
                users.Add(new Human(user_data));
            }
            // TODO Выбрать оптимальное значение 
            if (users.Count < 1000)
            {
                response_list_box.SetValue(ScrollViewer.CanContentScrollProperty, false);
            }
            else
            {
                response_list_box.SetValue(ScrollViewer.CanContentScrollProperty, true);
            }

            response_list_box.ItemsSource = users;
        }

        private void AddSelectedUsers(Human new_user)
        {
            // TODO При одноклассниках стоит использовать другую проверку
            if (selected_users.Where(x => x.id == new_user.id).ToList().Count() == 0)
            {
                selected_users.Add(new_user);
                selected_users_list_box.ItemsSource = selected_users;
            }
            else
            {
                Console.WriteLine("Данный пользователь уже находится в списке");
            }
        }

        private void RemoveSelectedUsers(Human new_user)
        {
            if (selected_users.Count > 0)
            {
                // TODO Это плохое решение задачи
                try
                {
                    // TODO При одноклассниках стоит использовать другую проверку
                    if (selected_users.Where(x => x.id == new_user.id).ToList().Count() == 1)
                    {
                        selected_users.Remove(new_user);
                        selected_users_list_box.ItemsSource = selected_users;
                    }
                    else
                    {
                        Console.WriteLine("Что-то пошло не так");
                    }
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /* Кнопки */
        private void ButtonMakeRequest(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сделать запрос");
            MakeRequest make_request = new MakeRequest();
            make_request.ShowDialog();
        }

        private void ButtonAddToList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Добавить в список");
            AddSelectedUsers(SelectedHuman);
        }

        private void ButtonRemoveFromList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Удалить из списка");
            RemoveSelectedUsers(SelectedListItem);
        }

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
        
        protected override void OnClosed(EventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Закрытие окна");
            App.Current.MainWindow.Show();
            base.OnClosed(e);
        }

        /* Временный метод пока нет бд */
        private static JArray LoadFileJson(string filename)
        {
            JArray data = new JArray();
            using (StreamReader file = File.OpenText(filename))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    data = JArray.Load(reader);
                }
            }
            return data;
        }

        // TODO Возможно стоит объеденить методы с прошлым меню
        /* Меню */
        private void MenuNewList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Новый список");
            Search search = new Search();
            this.Close();
            App.Current.MainWindow.Hide();
            search.Show();
        }

        private void MenuLoadList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Загрузить список");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            Console.WriteLine(this.ToString() + ": Загружен список: " + open_file_dialog.FileName);
        }

        private void MenuSaveList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сохранить список");
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();
            Console.WriteLine(this.ToString() + ": Сохранён список: " + save_file_dialog.FileName);
        }

        private void MenuLoadDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Загрузить БД");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Text files (*.json)|*.json|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            if (open_file_dialog.FileName != "")
            {
                UpdateResponse(open_file_dialog.FileName);
                Console.WriteLine(this.ToString() + ": Загружен список: " + open_file_dialog.FileName);
            }
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

        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
