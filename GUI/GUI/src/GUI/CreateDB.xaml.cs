﻿using System;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for CreateDB.xaml
    /// </summary>
    public partial class CreateDB : Window
    {
        private CheckBox local_groups;
        private CheckBox public_groups;
        private CheckBox search;

        public CreateDB()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            local_groups = (CheckBox) FindName("LocalGroups");
            public_groups = (CheckBox) FindName("PublicGroups");
            search = (CheckBox) FindName("Search");
        }

        // TODO Выводить информацию на экран
        private void ButtonCreateDB(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Сформировать БД :"
                + local_groups.IsChecked + " : " + public_groups.IsChecked + " : " + search.IsChecked);
            //new DBCreator().Create(local_groups.IsChecked.Value, public_groups.IsChecked.Value, search.IsChecked.Value);
            new DBCreator().Create(false, false, false);
            MessageBox.Show("Создание базы данных закончено", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            this.Close();
        }
    }
}
