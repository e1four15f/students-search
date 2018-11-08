﻿using System;
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
        private bool isMan;
        private bool isWoman;

        public MakeRequest()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Найти, Параметры: "
                + FirstName.Text + ":"
                + LastName.Text + ":"
                + (isMan ? isWoman ? "Мужчина:" : "Женщина:" : "Пол неопределён")
                + FacultyName.Text + ":"
                + ChairName.Text + ":"
                + GraduationYear.Text);
            this.Close();
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Очистить");
            FirstName.Text = "";
            LastName.Text = "";
            FacultyName.Text = "";
            ChairName.Text = "";
            GraduationYear.Text = "";
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