using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DB;

namespace GUI
{
    abstract class MenuController
    {
        /* Создаёт новую панель поиска */
        internal static void NewList(Window sender, bool saved = true)
        {
            Console.WriteLine(sender.ToString() + ": Новый список");
            if (sender != App.Current.MainWindow)
            { 
                sender.Close();
            }
            if (saved)
            {
                App.Current.MainWindow.Hide();
                new Search().Show();
            }
        }

        // TODO Доделать логику
        /* Загружает список */
        internal static void LoadList(Window sender, bool saved = true)
        {
            Console.WriteLine(sender.ToString() + ": Загрузить список");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            // TODO Придумать формат для файлов списка
            open_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            if (open_file_dialog.FileName != "")
            {
                if (sender != App.Current.MainWindow)
                {
                    sender.Close();
                } 
                if (saved)
                {
                    App.Current.MainWindow.Hide();
                    Console.WriteLine(sender.ToString() + ": Загружен список: " + open_file_dialog.FileName);
                    // TODO Читать файл и передавать его как новый параметр конструктору
                    new Search(System.IO.Path.GetFileNameWithoutExtension(open_file_dialog.FileName)).Show();
                }
            }
        }

        /* Вызывает окно сохранения списка и проверяет сохранён ли файл */
        internal static bool SaveList(Window sender, bool saved)
        {
            Console.WriteLine(sender.ToString() + ": Сохранить список");
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            // TODO Придумать расширения для файлов
            save_file_dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            save_file_dialog.ShowDialog();

            if (save_file_dialog.FileName.Count() != 0)
            {
                saved = true;
                sender.Title = saved ? sender.Title.Remove(sender.Title.Length - 1) : sender.Title + "*";
                Console.WriteLine(sender.ToString() + ": Сохранён список: " + save_file_dialog.FileName);
                return true;
            }
            else
            {
                Console.WriteLine(sender.ToString() + ": Отмена сохранения");
                return false;
            }
        }

        /* Загружает БД */
        internal static void LoadDB(Window sender)
        {
            Console.WriteLine(sender.ToString() + ": Загрузить БД");
            OpenFileDialog open_file_dialog = new OpenFileDialog();
            open_file_dialog.Filter = "Text files (*.json)|*.json|All files (*.*)|*.*";
            open_file_dialog.ShowDialog();
            if (open_file_dialog.FileName != "")
            {
                // TODO Нужно иметь общий объект AboutDB(db_users.Count);
                //MainWindow.db.LoadDB(open_file_dialog.FileName);
                Console.WriteLine(sender.ToString() + ": Загружена БД: " + open_file_dialog.FileName);
            }
            if (sender == App.Current.MainWindow)
            {
                new Search();
            }
        }

        /* Информация о БД */
        internal static void DBInfo(Window sender)
        {
            Console.WriteLine(sender.ToString() + ": Информация о БД");
            // TODO мб нужно добавить обьект бд с мета информацией и передавать его
            // TODO Нужно иметь общий объект AboutDB(db_users.Count);
            new AboutDB().ShowDialog();
        }

        /* Настройки */
        internal static void Settings(object sender)
        {
            Console.WriteLine(sender.ToString() + ": Настройки");
            new Settings().Show();
        }

        /* Выход */
        internal static void Exit(Window sender)
        {
            Console.WriteLine(sender.ToString() + ": Выход");
            sender.Close();
        }
    }
}
