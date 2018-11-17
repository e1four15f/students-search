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

using WebApi;
using DB;
using Microsoft.Win32;
using System.IO;
using System.Threading;

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
        }

        // TODO Выводить информацию на экран
        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сформировать БД :"
                + LocalGroups.IsChecked + " : " + PublicGroups.IsChecked + " : " + Search.IsChecked);

            if (!LocalGroups.IsChecked.Value && !PublicGroups.IsChecked.Value && !Search.IsChecked.Value)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один метод поиска", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.Filter = "Text files (*.ldb)|*.ldb|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();
            if (save_file_dialog.FileName != "")
            {
                Console.WriteLine(save_file_dialog.FileName);

                Thread loading_thread = new Thread(() => new Loading().Start());
                loading_thread.SetApartmentState(ApartmentState.STA);
                loading_thread.Start();

                new DBCreator().Create(save_file_dialog.FileName,
                    LocalGroups.IsChecked.Value, PublicGroups.IsChecked.Value, Search.IsChecked.Value, Friends.IsChecked.Value);
                
                loading_thread.Abort();
            }
            MessageBox.Show("Создание базы данных закончено!", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            this.Close();
        }

        private void CheckboxChange(object sender, RoutedEventArgs e)
        {
            if (!LocalGroups.IsChecked.Value && !PublicGroups.IsChecked.Value && !Search.IsChecked.Value)
            {
                Friends.IsEnabled = false;
                Friends.IsChecked = false;
            }
            else
            {
                Friends.IsEnabled = true;
            }

        }

        private void Temp(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.Filter = "Text files (*.ldb)|*.ldb|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();
            new DBCreator().Create(save_file_dialog.FileName);
        }
    }
}
