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

namespace GUI
{
    /// <summary>
    /// Interaction logic for MakeRequest.xaml
    /// </summary>
    public partial class MakeRequest : Window
    {
        public MakeRequest()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Найти");
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Очистить");
        }
    }
}
