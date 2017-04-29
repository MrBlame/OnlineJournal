using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Dao;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;

namespace Server
{
    public class GeneralDao
    {
        protected MySqlConnection connection { get; set; }
        protected List<DataTable> tablesToSend { get; set; }
        protected ServerStatus serverStatus { get; set; }
        protected string query { get; set; }

        protected string[] SelectObjectsListFromRequest(string request)
        {
            string[] result = request.Split(',');
            for (int i = 0; i < result.Count(); i++)
            {
                result[i] = result[i].Trim();
            }
            return result;
        }

        protected DataTable ReadDataFromDataBase()
        {
            DataTable table = new DataTable();
            table.Locale = new CultureInfo("uk-UA");
            MySqlCommand sqlCom = new MySqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            dataAdapter.Fill(table);

            ShowResults(table); // For console testing

            return table;
        }

        protected void InsertDataInDataBase()
        {
            MySqlCommand sqlCom = new MySqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            sqlCom.ExecuteNonQuery();
        }

        protected void ShowResults(DataTable tables)
        {
            Console.WriteLine("SELECT WAS EXECUTE");
            Console.Write("Result: ");
            for (int j = 0; j < tables.Rows.Count; j++)
            {
                DataRow row = tables.Rows[j];

                for (int n = 0; n < tables.Columns.Count; n++)
                {
                    DataColumn column = tables.Columns[n];

                    Console.Write(row[column] + ", ");
                }
                Console.WriteLine();
            }
        }

        protected void AddTableToResult()
        {
            DataTable table = ReadDataFromDataBase();
            if (table.Rows.Count == 0)
            {
                serverStatus = ServerStatus.DB_ERROR_IN_DATA;
                Console.WriteLine("Empty result");
            }
            else
            {
                tablesToSend.Add(table);
            }
        }

        public void WriteDataToFile(List<DataTable> tables)
        {
            int i = 0;
            string submittedFilePath = "C:/Users/Infamous/Desktop/Projects/Server/log.txt";
            StreamWriter sw = null;

            sw = new StreamWriter(submittedFilePath, false);

            for (int j = 0; j < tables.Count; j++)
            {
                for (i = 0; i < tables[j].Columns.Count - 1; i++)
                {

                    sw.Write(tables[j].Columns[i].ColumnName + "|");

                }
                sw.Write(tables[j].Columns[i].ColumnName);
                sw.WriteLine();

                foreach (DataRow row in tables[j].Rows)
                {
                    object[] array = row.ItemArray;

                    for (i = 0; i < array.Length - 1; i++)
                    {
                        sw.Write(array[i].ToString() + "|");
                    }
                    sw.Write(array[i].ToString());
                    sw.WriteLine();
                }
                sw.WriteLine();
            }

           
            sw.Close();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void SetConnection(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public List<DataTable> GetData()
        {
            return tablesToSend;
        }

        public ServerStatus GetCommand()
        {
            return serverStatus;
        }

        public void SetCommand(ServerStatus serverStatus)
        {
            this.serverStatus = serverStatus;
        }

    }
}
