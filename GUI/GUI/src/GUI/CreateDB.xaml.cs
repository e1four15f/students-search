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

namespace GUI
{
    /// <summary>
    /// Interaction logic for CreateDB.xaml
    /// </summary>
    public partial class CreateDB : Window
    {
        public CreateDB()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        // TODO Выводить информацию на экран
        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сформировать БД :"
                + LocalGroups.IsChecked + " : " + PublicGroups.IsChecked + " : " + Search.IsChecked);

            SaveFileDialog save_file_dialog = new SaveFileDialog();
            // TODO Придумать формат для файлов списка
            save_file_dialog.Filter = "Text files (*.json)|*.json|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();
            if (save_file_dialog.FileName != "")
            {
                Console.WriteLine(save_file_dialog.FileName);
                new DBCreator().Create(save_file_dialog.FileName,
                    LocalGroups.IsChecked.Value, PublicGroups.IsChecked.Value, Search.IsChecked.Value);
            }
            
            MessageBox.Show("Создание базы данных закончено", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            this.Close();
        }
    }
}
