using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        public Search()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void ButtonMakeRequest(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Сделать запрос");
            MakeRequest make_request = new MakeRequest();
            make_request.ShowDialog();
        }

        private void ButtonAddToList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Добавить в список");
        }

        private void ButtonRemoveFromList(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Удалить из списка");
        }

        private void ButtonMoreInfo(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Подробная информация");
        }
    }
}
