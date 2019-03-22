using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Table
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

        
    }
}
