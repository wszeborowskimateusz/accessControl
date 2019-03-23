using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class MessageInterpreter
    {

        private AccessControl accessControl;

        public MessageInterpreter(AccessControl ac)
        {
            accessControl = ac;
        }

        public string InterpretMessage(string message)
        {
            string[] messageParts = message.Split(' ');
            switch(messageParts[0])
            {
                case "AUTH":
                    if (messageParts.Length < 3) return "";
                    else return Auth(messageParts[1], messageParts[2]);
                case "GET":
                    if (messageParts.Length < 3) return "";
                    return GetTable(messageParts[1], messageParts[2]);
                case "SET":
                    if (messageParts.Length < 4) return "";
                    string[] arguments = new string[messageParts.Length - 3];
                    Array.Copy(messageParts, 3, arguments, 0, messageParts.Length - 3);
                    return Set(messageParts[1], messageParts[2], arguments);
                case "DEL":
                    if (messageParts.Length < 4) return "";
                    return Del(messageParts[1], messageParts[2], messageParts[3]);
                case "ADD":
                    if (messageParts.Length < 4) return "";
                    return Add(messageParts[1], messageParts[2], messageParts[3]);
                case "LOGOUT":
                    if (messageParts.Length < 2) return "";
                    return Logout(messageParts[1]);
                default: return "";
            }
        }

        private string Auth(string login, string pass)
        {
            string token = accessControl.UserLogIn(login, pass);
            if (token != "") return "AUTH_CONF " + token;
            else return "AUTH_DENY";
        }

        private string Authorize(string token, string tableName)
        {
            if (!accessControl.context.TablePermissions.Any(t => string.Compare(t.nameOfTable, tableName) == 0)) return "NO_PERMISSION";
            switch (accessControl.checkPermissions(token, tableName))
            {
                case AUTHORIZATION_RESPONSE.NO_PERMISSION:
                    return "NO_PERMISSION";
                case AUTHORIZATION_RESPONSE.NO_USER:
                    return "NO_USER";
                case AUTHORIZATION_RESPONSE.OK:
                    return "OK";
            }
            return "";
        }

        private string GetTable(string token, string tableName)
        {
            string authorize = Authorize(token, tableName);
            if (authorize != "OK") return authorize;

            return authorize + " " + GetTableContent(tableName);
        }

        private string GetTableContent(string tableName)
        {
            StringBuilder response = new StringBuilder();
            switch(tableName)
            {
                case "Article":
                    response.Append("name;description;producer_id ");
                    foreach(var raw in accessControl.context.Articles)
                    {
                        response.Append(raw.name + ";" + raw.description.Replace(' ', '+') + ";" + raw.producer_id + " ");
                    }
                    break;
                case "SpecificArticle":
                    response.Append("id;sale_id;price;article_id ");
                    foreach (var raw in accessControl.context.SpecificArticles)
                    {
                        response.Append(raw.id + ";" + raw.sale_id + ";" + raw.price + ";" + raw.article_id + " ");
                    }
                    break;
                case "Sale":
                    response.Append("id;netPrice;grossPrice;tax;customer_id ");
                    foreach (var raw in accessControl.context.Sales)
                    {
                        response.Append(raw.id + ";" + raw.netPrice + ";" + raw.grossPrice + ";" + raw.tax + ";" + raw.customer_id + " ");
                    }
                    break;
                case "Producer":
                    response.Append("id;name;address ");
                    foreach (var raw in accessControl.context.Producers)
                    {
                        response.Append(raw.id + ";" + raw.name + ";" + raw.address.Replace(' ', '+') + " ");
                    }
                    break;
                case "Customer":
                    response.Append("id;name;surname ");
                    foreach (var raw in accessControl.context.Customers)
                    {
                        response.Append(raw.id + ";" + raw.name + ";" + raw.surname + " ");
                    }
                    break;
            }
            if (response[response.Length - 1].CompareTo(' ') == 0) response.Length--;
            return response.ToString();
        }

        private string Set(string token, string tableName, string[] listOvUpdates)
        {
            string authorize = Authorize(token, tableName);
            if (authorize != "OK") return authorize;

            StringBuilder sqlCommand = new StringBuilder();
            foreach (var update in listOvUpdates)
            {
                sqlCommand.Clear();

                sqlCommand.Append("UPDATE " + tableName + " SET ");

                foreach (var argument in update.Split(';'))
                {
                    string[] row = argument.Split('=');
                    string rowId = row[0];
                    string colName = row[1];
                    string colVal = row[2];
                    if (IsStringColumn(colName))
                        sqlCommand.Append(colName + "='" + colVal + "'");
                    else
                        sqlCommand.Append(colName + "=" + colVal);
                    if (string.Compare(tableName, "Article") == 0)
                        sqlCommand.Append(" WHERE name='" + rowId + "';");
                    else
                        sqlCommand.Append(" WHERE id=" + rowId + ";");
                }

                accessControl.context.Database.ExecuteSqlCommand(sqlCommand.ToString());
            }

            return "OK";
        }

        private string Del(string token, string tableName, string rowId)
        {
            string authorize = Authorize(token, tableName);
            if (authorize != "OK") return authorize;

           
            if(string.Compare(tableName, "Article") == 0)
                accessControl.context.Database.ExecuteSqlCommand("DELETE FROM " + tableName + " WHERE name='" + rowId + "';");
            else
                accessControl.context.Database.ExecuteSqlCommand("DELETE FROM " + tableName + " WHERE id=" + rowId + ";");

            return "OK";
        }

        private string Add(string token, string tableName, string arguments)
        {
            string authorize = Authorize(token, tableName);
            if (authorize != "OK") return authorize;

            StringBuilder columnNames = new StringBuilder();
            StringBuilder columnValues = new StringBuilder();

            foreach (var argument in arguments.Split(';'))
            {
                string[] pair = argument.Split('=');
                if(IsStringColumn(pair[0]))
                    columnValues.Append("'" + pair[1] + "',");
                else columnValues.Append(pair[1] + ",");
                columnNames.Append(pair[0] + ",");

            }
            if(columnNames[columnNames.Length - 1].CompareTo(',') == 0)
                columnNames.Length--;
            if (columnValues[columnValues.Length - 1].CompareTo(',') == 0)
                columnValues.Length--;


            accessControl.context.Database.ExecuteSqlCommand("INSERT INTO " + tableName + "(" + columnNames.ToString() + ") VALUES (" + columnValues.ToString() + ");" );

            return "OK";
        }

        private string Logout(string token)
        {
            accessControl.Logout(token);
            return "OK";
        }

        private bool IsStringColumn(string columnName)
        {
            if (string.Compare(columnName, "name") == 0
                || string.Compare(columnName, "surname") == 0
                || string.Compare(columnName, "description") == 0
                || string.Compare(columnName, "address") == 0)
                return true;
            else return false;
        }
    }
}
