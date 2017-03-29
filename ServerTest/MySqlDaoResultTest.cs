using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using System.Data;



namespace ServerTest
{
    [TestClass]
    public class MySqlDaoResultTest
    {
        private MySqlConnection TestConnection()
        {
            string serverName = "localhost";
            string userName = "root";
            string dbName = "universitydb";
            string port = "3306";
            string password = "IwtFbhGh_71";
            string connection = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            return new MySqlConnection(connection);
        }

        [TestMethod]
        public void TestLogin()
        {
            string[] testCommand = { "Login", "Gresko", "1234" };
            MySqlDao mySqlDao = new MySqlDao(TestConnection());
            DataTable expectedTable = new DataTable();
            expectedTable.Columns.Add("ID", typeof(Int32));
            DataRow row = expectedTable.NewRow();
            row["ID"] = 1;
            expectedTable.Rows.Add(row);

            mySqlDao.CreateQuerry(testCommand);

           // Assert.AreEqual(1, mySqlDao.GetData()[0].Rows.Count);
            Assert.AreSame(expectedTable, mySqlDao.GetData()[0]);
           // Assert.IsTrue(DataRowComparer)
                

        }
    }
}
