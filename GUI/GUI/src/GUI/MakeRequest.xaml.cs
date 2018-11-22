using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MakeRequest.xaml
    /// </summary>
    public partial class MakeRequest : Window
    {
        //private string current_placeholder;

        private Array faculArr { get; set; }

        public class Chair
        {
            [JsonProperty("title")]
            public string Name;
            [JsonProperty("id")]
            public int Value;
            public Chair(string name, int value)
            {
                Name = name;
                Value = value;
            }
            public override string ToString()
            {
                return Name.Trim(new char[] { ' ' });
            }
        }

        public class Faculty
        {
            [JsonProperty("title")]
            public string Name { get; set; }
            [JsonProperty("id")]
            public int Value { get; set; }
            [JsonProperty("chairs")]
            public Chair[] ChairList { get; set; }
            public Faculty(string name, int value)
            {
                Name = name;
                Value = value;
            }
            public override string ToString()
            {
                return Name.Trim(new char[] { ' ' });
            }
        }

        public MakeRequest()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            string json = Encoding.UTF8.GetString(Properties.Resources.fac_cha);

            Faculty[] facultArr = JsonConvert.DeserializeObject<Faculty[]>(json);
            FacultyName.Items.Add(new Faculty("", -1));
            foreach (Faculty faculty in facultArr)
            {
                FacultyName.Items.Add(faculty);
            }
            faculArr = new Faculty[FacultyName.Items.Count];
            FacultyName.Items.CopyTo(faculArr, 0);

            FacultyName.SelectionChanged += facultyChanged;
            FacultyName.KeyUp += facultyPreviewControlKey;

            ChairName.Items.Add(new Faculty("Сначала выберите факультет", -1));
            ChairName.KeyUp += chairPreviewControlKey;
        }

        // EventHandlers
        private void facultyChanged(object sender, SelectionChangedEventArgs args)
        {
            ChairName.Items.Clear();

            Faculty facult = (Faculty)FacultyName.SelectedItem;
            if (facult != null && facult.Value != -1)
            {
                foreach (Chair chair in facult.ChairList)
                {
                    ChairName.Items.Add(chair);
                }
            }
            else
            {
                ChairName.Items.Add(new Faculty("Сначала выберите факультет", -1));
            }    
        }

        private void facultyPreviewControlKey(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Up && e.Key != Key.Enter)
            {
                FacultyName.Items.Clear();

                foreach (Faculty fac in faculArr)
                {
                    if (fac.Name.ToUpper().Contains(FacultyName.Text.ToUpper()))
                    {
                        FacultyName.Items.Add(fac);
                    }
                }

                FacultyName.IsDropDownOpen = true;
            }        
        }

        private void chairPreviewControlKey(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down && e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Up && e.Key != Key.Enter)
            {
                Faculty currFaculty = FacultyName.SelectedItem as Faculty;
                if (currFaculty != null && currFaculty.Value != -1)
                {
                    ChairName.Items.Clear();

                    foreach (Chair chair in currFaculty.ChairList)
                    {
                        if (chair.Name.ToUpper().Contains(ChairName.Text.ToUpper()))
                        {
                            ChairName.Items.Add(chair);
                        }
                    }

                    ChairName.IsDropDownOpen = true;
                }
                else
                {
                    ChairName.Items.Clear();
                    ChairName.Items.Add(new Faculty("Сначала выберите факультет", -1));
                }
            }
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Найти, Параметры: "
                + FirstName.Text + ":"
                + LastName.Text + ":"
                + (ManSex.IsChecked.Value ? FemaleSex.IsChecked.Value ? "Мужчина:" : "Женщина:" : "Пол неопределён")
                + FacultyName.Text + ":"
                + ChairName.Text + ":"
                + GraduationYear.Text);

            this.Close();
        }

        // KeyListeners
        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Очистить");

            FirstName.Text = "";
            LastName.Text = "";
            FacultyName.Text = "";
            ChairName.Text = "";
            GraduationYear.Text = "";

            ManSex.IsChecked = false;
            FemaleSex.IsChecked = false;
        }

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
