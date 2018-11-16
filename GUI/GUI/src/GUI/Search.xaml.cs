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

using RuntimePlugin_ns;
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

        private bool saved;

        public Search(string current_file = "Новый список", List<Human> selected_users = null)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            if (selected_users == null)
            {
                this.selected_users = new ObservableCollection<Human>();
                UpdateResponse(MainWindow.db_users);
            }
            else
            {
                this.selected_users = new ObservableCollection<Human>(selected_users);
                UpdateResponse(selected_users);
                SelectedUsersListBox.ItemsSource = this.selected_users;
            }

            saved = true;
            this.Title = current_file;
        }

        // TODO Разобраться с ошибкой ArgumentOutOfRange
        /* Обновляет окно результатов поиска пользователей */
        private void UpdateResponse(List<Human> users_from_db)
        {
            response_users = new ObservableCollection<Human>(users_from_db); 

            // TODO Выбрать оптимальное значение 
            if (response_users.Count < 1000)
            {
                ResponseListBox.SetValue(ScrollViewer.CanContentScrollProperty, false);
            }
            else
            {
                ResponseListBox.SetValue(ScrollViewer.CanContentScrollProperty, true);
            }

            ResponseInfo.Content = "Найденно " + response_users.Count + " пользователей";
            ResponseListBox.ItemsSource = response_users;
        }

        /* Вызывает окно сохранения списка и проверяет сохранён ли файл */
        private bool SaveListDialog()
        {
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            // TODO Придумать расширения для файлов
            save_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();

            if (save_file_dialog.FileName.Count() != 0)
            {
                saved = true;
                this.Title = saved ? this.Title.Remove(this.Title.Length - 1) : this.Title + "*";
                Console.WriteLine(this.ToString() + ": Сохранён список: " + save_file_dialog.FileName);
                return true;
            }
            else
            {
                Console.WriteLine(this.ToString() + ": Отмена сохранения");
                return false;
            }
        }

        /* Кнопки */
        /* Открывает окно создания запроса к бд */
        private void ButtonMakeRequest(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сделать запрос");
            MakeRequest make_request = new MakeRequest();
            make_request.ShowDialog();

            // UPD Биде есть
            if (MainWindow.db != null)
            { 
                List<Human> criterion_users = MainWindow.db.search(make_request);
                Console.WriteLine(criterion_users.Count);
                //if (criterion_users.Count != MainWindow.db_users.Count)
                //{
                UpdateResponse(criterion_users);
                //}
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
                // UPD Для БД нужен был свой индекс типа ObjectId, здесь использую его
                // TODO Сделать нормальную проверку, мб переопределить == в хумане 
                if (!selected_users.Any(x => x == SelectedHuman))
                {
                    selected_users.Add(SelectedHuman);
                    SelectedUsersListBox.ItemsSource = selected_users;
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
                    // UPD Для БД нужен был свой индекс типа ObjectId, здесь использую его
                    if (!SelectedListItem.Equals(null))
                    {
                        selected_users.Remove(selected_users.Single(x => x == SelectedListItem));
                        //selected_users.Remove(selected_users.Single(x => x.id == SelectedListItem.id));
                        SelectedUsersListBox.ItemsSource = selected_users;
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

        /* Открывает окно для дополнительных функция для работы со списком */
        private void ButtonListOperation(object sender, RoutedEventArgs e)
        {
            new ListOperation(selected_users.ToList()).ShowDialog();
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
                    if (MenuController.SaveList(this, saved, selected_users.ToList()))
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
            saved = MenuController.SaveList(this, saved, selected_users.ToList());
        }

        /* Загружает БД */
        private void MenuLoadDB(object sender, RoutedEventArgs e)
        {
            MenuController.LoadDB(this);
            UpdateResponse(MainWindow.db_users);
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

        /* Создатели */
        private void About(object sender, RoutedEventArgs e)
        {
            MenuController.About(this);
        }

        /* Drag and Drop обработчики */
        private void ResponseListBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is DockPanel)
                {
                    DockPanel draggedItem = sender as DockPanel;
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                }
            }
        }
        
        HashSet<RuntimePlugin> plugins = new HashSet<RuntimePlugin>();
        
        /* Создатели */
        private void PluginLoad(object sender, RoutedEventArgs e)
        {
        	OpenFileDialog dialog = new OpenFileDialog();
        	dialog.ShowDialog();
        	if(dialog.FileName.Contains(".dll"))
        		plugins.Add(new RuntimePlugin(dialog.FileName));
        }
        
        private void LaunchPlugin(object sender, RoutedEventArgs e)
        {
        	List<Human> humans = selected_users.ToList();
        	string returned = " ";
        	if(plugins.Count != 0)
        		plugins.ElementAt(0).Call(humans);
        }
        
    }
}
