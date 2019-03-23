using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Table
    {
        public List<List<string>> listOfRows;

        public string tableName;
        public List<string> listOfColumns;

        public Table(string name, List<string> columns, List<List<string>> raws)
        {
            tableName = name;
            listOfColumns = columns;
            listOfRows = raws;

        }

        public void PrintTable()
        {

            Console.Out.WriteLine("Table name: {0}", tableName);
            Console.Out.Write("( ");
            listOfColumns.ForEach(c => Console.Out.Write(c + " "));
            Console.Out.WriteLine(" )");

            listOfRows.ForEach(raw => {
                Console.Out.Write("( ");
                raw.ForEach(r => Console.Out.Write(r + " "));
                Console.Out.WriteLine(" )");
            });
        }
    }
}
