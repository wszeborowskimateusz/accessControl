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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //SslTcpClient c = new SslTcpClient();
            //DataManipulator data = new DataManipulator(c);
            //string response = data.LogIn("john", "alamakota");
            //Console.Out.WriteLine("Message from a server is: {0}", response);
            ////Table t = data.GetTable(response, "Customer");
            ////t.PrintTable();
            ////data.DelRow(response, "Customer", "1");
            ////data.AddRow(response, "Customer", "id=1", "name=Jan", "surname=Nowak");
        }

        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPass.Password;
            SslTcpClient c = new SslTcpClient();
            DataManipulator data = new DataManipulator(c);
            string token = data.LogIn(login, password);
            if (token.CompareTo("") != 0)
            {
                DataManipulation win2 = new DataManipulation(token, data);
                win2.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Credentials incorrect");
            }
        }
    }
}
