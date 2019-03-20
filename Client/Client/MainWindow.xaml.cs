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
            SslTcpClient c = new SslTcpClient();
            string response = c.SendMessage("AUTH jan alamakota");
            Console.Out.WriteLine("Message from a server is: {0}", response);
            response = c.SendMessage("GET " + response.Split(' ')[1] + " Customer");
            Console.Out.WriteLine("Message from a server is: {0}", response);
        }
    }
}
