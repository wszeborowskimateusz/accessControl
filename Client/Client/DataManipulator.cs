using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class DataManipulator
    {
        private SslTcpClient tcpClient;

        public DataManipulator(SslTcpClient c)
        {
            tcpClient = c;
        }

        //If logging in succeeds this method returns user token that is needed for further data access
        //Else it returns an empty string
        public string LogIn(string login, string password)
        {
            string response = tcpClient.SendMessage("AUTH " + login + " " + password);

            string[] responseValues = response.Split(' ');

            if (responseValues.Length > 0 && responseValues[0].CompareTo("AUTH_DENY") == 0)
                return "";
            else if (responseValues.Length > 1 && responseValues[0].CompareTo("AUTH_CONF") == 0)
                return responseValues[1];

            return "";
        }

        public void LogOut(string token)
        {
            tcpClient.SendMessage("LOGOUT " + token);
        }

        public Table GetTable(string token, string tableName)
        {
            string response = tcpClient.SendMessage("GET " + token + " " + tableName);
            string[] responseValues = response.Split(' ');
            SERVER_RESPONSE responseCode = stringToResponse(responseValues[0]);
            if (responseCode != SERVER_RESPONSE.OK || responseValues.Length < 3)
                return null;

            List<string> listOfColumns = new List<string>();
            foreach(var column in responseValues[1].Split(';'))
            {
                listOfColumns.Add(column);
            }

            List<List<string>> listOfRaws = new List<List<string>>();
            for (int i = 2; i < responseValues.Length; i++)
            {
                List<string> rawValues = new List<string>();
                foreach (var raw in responseValues[i].Split(';'))
                {
                    rawValues.Add(raw);
                }
                listOfRaws.Add(rawValues);
            }

            return new Table(tableName, listOfColumns, listOfRaws);
        }

        // SET token tableName rowid1=colName1=colValue1 rowid2=colName2=colValue2 rowidN=colNameN=colValueN 
        // Each argument in the last parameter must be set at a given format: rowid=colName=colValue
        public SERVER_RESPONSE SetTable(string token, string tableName, params string[] arguments)
        {
            if (arguments.Length < 1)
                return SERVER_RESPONSE.ERROR;

            StringBuilder serverRequest = new StringBuilder();

            serverRequest.Append("SET " + token + " " + tableName + " ");

            foreach (var argument in arguments)
            {
                serverRequest.Append(argument + " ");
            }

            if (serverRequest[serverRequest.Length - 1].CompareTo(' ') == 0)
                serverRequest.Length--;

            string response = tcpClient.SendMessage(serverRequest.ToString());

            return stringToResponse(response);
        }

        public SERVER_RESPONSE DelRow(string token, string tableName, string rowId)
        {
            string response = tcpClient.SendMessage("DEL " + token + " " + tableName + " " + rowId);

            return stringToResponse(response);
        }

        // ADD token tableName colName1=colVal1;colName2=colVal2;colNameN=colValN;
        // To arguments parameter we pass key value pairs, where key is column name and value is value pair
        // You need to pass it in a given format: key=value
        public SERVER_RESPONSE AddRow(string token, string tableName, params string[] arguments)
        {
            if(arguments.Length < 1)
                return SERVER_RESPONSE.ERROR;

            StringBuilder serverRequest = new StringBuilder();

            serverRequest.Append("ADD " + token + " " + tableName + " ");

            foreach(var argument in arguments)
            {
                serverRequest.Append(argument + ";");
            }

            if (serverRequest[serverRequest.Length - 1].CompareTo(';') == 0)
                serverRequest.Length--;

            string response = tcpClient.SendMessage(serverRequest.ToString());

            return stringToResponse(response);
        }

        private SERVER_RESPONSE stringToResponse(string response)
        {
            if (response.CompareTo("OK") == 0)
                return SERVER_RESPONSE.OK;
            else if (response.CompareTo("NO_PERMISSION") == 0)
                return SERVER_RESPONSE.NO_PERMISSION;
            else if (response.CompareTo("NO_USER") == 0)
                return SERVER_RESPONSE.NO_USER;

            return SERVER_RESPONSE.ERROR;
        }
    }
}
