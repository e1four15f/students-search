using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Navigation;

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
        private List<Human> checked_users;

        public Human SelectedListItem { get; set; }

        private bool saved;

        public Search(string current_file = "Новый список", List<Human> selected_users = null)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            MessageInfo.Content = "Загружена бд " + Path.GetFileNameWithoutExtension(MainWindow.db_name);

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

            ResponseInfo.Content = "Найдено " + response_users.Count + " пользователей";
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
            
            try
            {
                checked_users = ResponseListBox.SelectedItems.Cast<Human>().ToList();

                foreach (Human human in checked_users)
                {
                    if (!selected_users.Any(x => x._id == human._id))
                    {
                        selected_users.Add(human);
                        SelectedUsersListBox.ItemsSource = selected_users;
                        if (saved)
                        {
                            this.Title += "*";
                        }
                        saved = false;
                        MessageInfo.Content = "";
                    }
                    else
                    {
                        MessageInfo.Content = "Данный пользователь уже находится в списке";
                        Console.WriteLine("Данный пользователь уже находится в списке");
                    }
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
                MessageBox.Show(ResponseListBox.SelectedItems.Cast<Human>().ToList()[0].ToString());
            } 
            catch (ArgumentOutOfRangeException ex)
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
        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
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
            string connStr = ConfigurationManager.AppSettings.Get("db_destination");
            MessageInfo.Content = "Загружена бд " + Path.GetFileNameWithoutExtension(connStr);    
        }

        /* Информация о БД */
        private void MenuDBInfo(object sender, RoutedEventArgs e)
        {
            MenuController.DBInfo(this);
        }

        /* Печать */
        private void MenuPrintList(object sender, RoutedEventArgs e)
        {
            MenuController.MenuPrintList(this, selected_users.ToList());
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
        	ObservableCollection<RuntimePlugin> plugins2watch = new ObservableCollection<RuntimePlugin>(plugins.ToList());
        	//GUI.PluginManager open_manager = new GUI.PluginManager();
        	
        	//open_manager.Show();
        	if(plugins.Count != 0)
        		plugins.ElementAt(0).Call(humans);
        }
        
        private void MakeTemplate(object sender, RoutedEventArgs e)
        {
        	System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
        	folder.ShowDialog();
        	RuntimePlugin.CreateTemplate(folder.SelectedPath);
        	MessageBox.Show("Файлы созданы");
        }

        /* Обработчик чекбоксов */
        private void IsSelectedCheckboxChange(object sender, RoutedEventArgs e)
        {
            
        }

        private IEnumerable<DependencyObject> FindContolsByType(DependencyObject dObject, Type targetType)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(dObject);
            var list = new List<DependencyObject>();
            for (int i = 0; i < childCount; i++)
            {
                var control = VisualTreeHelper.GetChild(dObject, i);
                if (control.GetType() == targetType)
                {
                    list.Add(control);
                }
                if (VisualTreeHelper.GetChildrenCount(control) > 0)
                {
                    list.AddRange(FindContolsByType(control, targetType));
                }
            }

            return list;
        }

        private void ButtonUpdatePlausibility(object sender, RoutedEventArgs e)
        {
            Parallel.ForEach(MainWindow.db_users, user =>
            {
                user.CalcPlausibility();
            });

            List<Human> users = MainWindow.db_users;
            string connStr = MainWindow.db.connStr;

            MainWindow.db = new DatabaseAPI();
            MainWindow.db.saveDB(connStr);
            MainWindow.db.addUsers(users);
            MainWindow.db_users = MainWindow.db.getAllUsers();

            UpdateResponse(MainWindow.db_users);
            MessageBox.Show("Готово", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
