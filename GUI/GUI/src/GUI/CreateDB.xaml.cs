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
using Microsoft.Win32;
using System.IO;
using System.Threading;

using Newtonsoft.Json.Linq;

using WebApi;
using DB;

namespace GUI
{
    /// <summary>
    /// Interaction logic for CreateDB.xaml
    /// </summary>
    public partial class CreateDB : Window
    {
        public CreateDB()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            if (MainWindow.db_users.Count() == 0)
            {
                UpdateDB.Visibility = Visibility.Collapsed;
            }
        }

        // TODO Выводить информацию на экран
        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            // Чекбоксы вк
            bool vk_local_groups = VkLocalGroups.IsChecked.Value;
            bool vk_public_groups = VkPublicGroups.IsChecked.Value;
            bool vk_search = VkSearch.IsChecked.Value;
            bool vk_friends = VkFriends.IsChecked.Value;

            // Чекбоксы ок
            bool ok_communities = OkCommunities.IsChecked.Value;
            bool ok_group = OkGroup.IsChecked.Value;
            
            Console.WriteLine(this.ToString() + ": Сформировать БД :"
                + vk_local_groups + " : " + vk_public_groups + " : " + vk_search + " : " + vk_friends + " : "
                + ok_communities + " : " + ok_group);

            if (!vk_local_groups && !vk_public_groups && !vk_search
                && !ok_communities && !ok_group)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один метод поиска", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.Filter = "Database files (*.ldb)|*.ldb|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();
            if (save_file_dialog.FileName != "")
            {
                Console.WriteLine(save_file_dialog.FileName);

                Thread loading_thread = new Thread(() => new Loading().Start());
                loading_thread.SetApartmentState(ApartmentState.STA);
                loading_thread.Start();

                new DBCreator().Create(save_file_dialog.FileName,
                    vk_local_groups, vk_public_groups, vk_search, vk_friends,
                    ok_communities, ok_group);
                
                loading_thread.Abort();
            }
            MessageBox.Show("Создание базы данных завершено!", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            this.Close();
        }

        private void CheckboxChange(object sender, RoutedEventArgs e)
        {
            if (!VkLocalGroups.IsChecked.Value && !VkPublicGroups.IsChecked.Value && !VkSearch.IsChecked.Value)
            {
                VkFriends.IsEnabled = false;
                VkFriends.IsChecked = false;
            }
            else
            {
                VkFriends.IsEnabled = true;
            }

        }

        private void ButtonUpdateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender.ToString() + ": Обновить бд");
            SaveFileDialog save_file_dialog = new SaveFileDialog();

            save_file_dialog.Filter = "Database files (*.ldb)|*.ldb|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();

            if (save_file_dialog.FileName.Count() != 0)
            {
                List<string> users_ids = new List<string>();

                // Получаем id пользователей из исходной бд
                Console.WriteLine("Before : " + MainWindow.db_users.Count());
                foreach (Human user in MainWindow.db_users)
                {
                    users_ids.Add(user.vk_id.ToString());
                }
                Console.WriteLine("After : " + users_ids.Count());

                // Получаем новую информацию через vk api
                JArray users_data = VkApiMulti.UsersGet(users_ids, VkApiUtils.fields, "Update");

                // Парсим json в Human
                List<Human> users = new List<Human>();
                Parallel.ForEach(users_data, user_data =>
                {
                    lock (users)
                    {
                        users.Add(new Human(user_data));
                    }
                });

                // Сохраняем данные в бд
                MainWindow.db = new DatabaseAPI();
                MainWindow.db.saveDB(save_file_dialog.FileName);
                MainWindow.db.addUsers(users);
                MainWindow.db_users = MainWindow.db.getAllUsers();

                MessageBox.Show("Обновление базы данных завершено!", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Console.WriteLine(sender.ToString() + ": Обнавлена бд: " + save_file_dialog.FileName);
                this.Close();
            }
            else
            {
                Console.WriteLine(sender.ToString() + ": Отмена обновления бд");
            }
        }
    }
}
