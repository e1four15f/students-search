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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void ButtonChangeLocalGroupsKeywords(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Изменить список ключевых слов для поиска локальных групп");
        }

        private void ButtonChangePublicGroupsKeywords(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Кнопка: Изменить список ключевых слов для поиска публичных групп");
        }
    }
}
