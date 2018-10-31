using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private Label result;
        private TextBox token;

        public Login()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            result = (Label) FindName("Result");
            token = (TextBox) FindName("Token");
        }

        private void ButtonAuth(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.ToString() + ": Авторизация");
            if (VkApiUtils.Auth(token.Text))
            {
                Console.WriteLine(this.ToString() + ": Успешная авторизация");
                result.Content = "Успешно!";
                result.Foreground = new SolidColorBrush(Colors.Green);
                this.Close();
            }
            else
            {
                Console.WriteLine(this.ToString() + ": Ошибка авторизации");
                result.Content = "Ошибка!";
                result.FontSize = 20;
                result.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://oauth.vk.com/authorize?client_id="
                + VkApiUtils.client_id
                + "&display=page&redirect_uri=http://vk.com&scope=offline&response_type=token&v="
                + VkApiUtils.version));
            e.Handled = true;
        }
    }
}
