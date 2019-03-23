using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for DataManipulation.xaml
    /// </summary>
    /// 
    public class CustomTable
    {
        public Dictionary<string, object> Custom { get; set; }

        public CustomTable()
        {
            Custom = new Dictionary<string, object>();
        }
    }

    public partial class DataManipulation : Window
    {
        private string token;
        private DataManipulator dataManipulator;
        private string[] tableNames = new string[] { "Article", "Customer", "Producer", "Sale", "SpecificArticle" };
        private List<Table> listOfTables;

        public DataManipulation(string token, DataManipulator dataManipulator)
        {
            InitializeComponent();
            this.token = token;
            this.dataManipulator = dataManipulator;
            listOfTables = new List<Table>();

            foreach(var tableName in tableNames)
            {
                Table table = dataManipulator.GetTable(token, tableName);
                if (table != null)
                {
                    AddTableTab(table);
                    listOfTables.Add(table);
                }
            }
        }

        void AddTableTab(Table t, bool refresh = false, TabItem tab = null)
        {
            //Create new tab
            TabItem table = new TabItem();
            table.Header = t.tableName;

            //Create new grid for a tab
            Grid grid = new Grid();

            if (refresh)
                table = tab;

            DataGrid dataGrid = new DataGrid();
            dataGrid.IsReadOnly = false;
            dataGrid.AutoGenerateColumns = false;

            foreach (var column in t.listOfColumns)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = column;
                textColumn.Width = 100;
                textColumn.Binding = new Binding("Custom[" + column + "]");
                dataGrid.Columns.Add(textColumn);
            }

            CustomTable[] tableArray = new CustomTable[t.listOfRows.Count];
            int i = 0;
            foreach (var raw in t.listOfRows)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                tableArray[i] = new CustomTable();
                int j = 0;
                foreach (var rawVal in raw)
                {
                    dict.Add(t.listOfColumns[j++], rawVal);

                }
                tableArray[i++].Custom = dict;
            }
            dataGrid.ItemsSource = tableArray;


            grid.Children.Add(dataGrid);

            Button deleteBbutton = new Button();
            deleteBbutton.Content = "Delete";
            deleteBbutton.VerticalAlignment = VerticalAlignment.Bottom;
            deleteBbutton.HorizontalAlignment = HorizontalAlignment.Right;
            deleteBbutton.Margin = new Thickness(100, 150, 10, 10);
            deleteBbutton.Click += (object sender, RoutedEventArgs e) => DeleteButton_Click(sender, e, t);
            grid.Children.Add(deleteBbutton);

            Button addButton = new Button();
            addButton.Content = "Add";
            addButton.VerticalAlignment = VerticalAlignment.Bottom;
            addButton.HorizontalAlignment = HorizontalAlignment.Right;
            addButton.Margin = new Thickness(100, 100, 100, 10);
            addButton.Click += (object sender, RoutedEventArgs e) => AddButton_Click(sender, e, t); 
            grid.Children.Add(addButton);

            table.Content = grid;

            if(!refresh)
                TabsControl.Items.Add(table);
        }

        private Window CreateAddWindow(Table table)
        {
            var window = new Window();
            window.Width = 350;
            window.Height = 350;
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            NameScope.SetNameScope(stackPanel, new NameScope());
            foreach (var column in table.listOfColumns)
            {
                stackPanel.Children.Add(new Label { Content = column });
                TextBox textbox = new TextBox();

                textbox.Name = column;
                stackPanel.RegisterName(textbox.Name, textbox);
                stackPanel.Children.Add(textbox);
            }
            Button submitButton = new Button { Content = "Submit" };
            submitButton.Click += (object sender, RoutedEventArgs e) => {
                List<string> arguments = new List<string>();
                foreach (var column in table.listOfColumns)
                {
                    TextBox textBox = (TextBox)stackPanel.FindName(column);
                    arguments.Add(column + "="  + textBox.Text.Replace(' ', '+'));

                }
                arguments.ForEach(Console.Out.WriteLine);
                dataManipulator.AddRow(token, table.tableName, arguments.ToArray());
                refreshTab(table.tableName);
                window.Close();
            };
            stackPanel.Children.Add(submitButton);
            
            window.Content = stackPanel;
            return window;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e, Table table)
        {
            Window window = CreateAddWindow(table);
            window.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e, Table table)
        {
            var window = new Window();
            window.Width = 350;
            window.Height = 350;
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            NameScope.SetNameScope(stackPanel, new NameScope());

            stackPanel.Children.Add(new Label { Content = "Row id" });
            TextBox textbox = new TextBox();

            textbox.Name = "rowid";
            stackPanel.RegisterName(textbox.Name, textbox);
            stackPanel.Children.Add(textbox);
            
            Button submitButton = new Button { Content = "Submit" };
            submitButton.Click += (object sender2, RoutedEventArgs e2) => {
        
                TextBox textBox = (TextBox)stackPanel.FindName("rowid");

                
                dataManipulator.DelRow(token, table.tableName, textBox.Text);
                refreshTab(table.tableName);
                window.Close();
            };
            stackPanel.Children.Add(submitButton);

            window.Content = stackPanel;
            window.ShowDialog();
        }

        private void refreshTab(string tableName)
        {
            TabItem tab = TabsControl.Items.OfType<TabItem>().SingleOrDefault(n => n.Header == tableName);
            if(tab != null)
            {
                tab.Content = null;
                AddTableTab(dataManipulator.GetTable(token, tableName), true, tab);
            }
        }

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            LogOut();
        }

        private void LogOutButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void LogOut()
        {
            dataManipulator.LogOut(token);
        }
    }
}
