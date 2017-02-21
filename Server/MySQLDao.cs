using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Server
{
    class MySqlDao
    {
        private MySqlConnection connection;


        public MySqlDao(MySqlConnection connection)
        {
            this.connection = connection;

        }

        public DataTable RunQuery(string query) {
            DataTable table = new DataTable();
            MySqlCommand sqlCom = new MySqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            
            sqlCom.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            dataAdapter.Fill(table);
            return table;
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
}
