    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        //private string current_placeholder;
        private TextBox first_name;
        private TextBox last_name;
        private TextBox faculty_name;
        private TextBox chair_name;
        private TextBox graduation_year;

        private bool isMan;
        private bool isWoman;

        public MakeRequest()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            first_name = (TextBox) FindName("FirstName");
            last_name = (TextBox) FindName("LastName");
            faculty_name = (TextBox) FindName("FacultyName");
            chair_name = (TextBox) FindName("ChairName");
            graduation_year = (TextBox) FindName("GraduationYear");
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Найти, Параметры: "
                + first_name.Text + ":"
                + last_name.Text + ":"
                + (isMan ? isWoman ? "Мужчина:" : "Женщина:" : "Пол неопределён")
                + faculty_name.Text + ":"
                + chair_name.Text + ":"
                + graduation_year.Text);
            this.Close();
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Очистить");
            first_name.Text = "";
            last_name.Text = "";
            faculty_name.Text = "";
            chair_name.Text = "";
            graduation_year.Text = "";
        }
        // Радиобаттоны не нужны
        /*
        private void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Переключатель");
            RadioButton radio_button = (RadioButton) sender;
            sex = radio_button.Content.ToString() == "Мужчина" ? true : false;
        }
        */
        private void TextNumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // TODO Сделать плейсхолдеры для полей 
        /*
        private void TextPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("TextPreviewMouseUp " + sender);
            TextBox text_box = (TextBox) sender;
            current_placeholder = text_box.Text;
            text_box.Text = "";
        }

        private void TextLostFocus(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("TextLostFocus " + sender);
            TextBox text_box = (TextBox) sender;
            if (text_box.Text == "")
            {
                text_box.Text = current_placeholder;
            }
        }
        */ 
    }
}
