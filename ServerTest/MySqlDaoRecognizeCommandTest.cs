using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server;
using MySql.Data.MySqlClient;

namespace ServerTest
{
    [TestClass]
    public class MySqlDaoRecognizeCommandTest
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
        public void TestRecognizeCommandForShowMonthSchedule()
        {
            string[] testCommand = { "ShowScheduleForMonth", "5", "2017-3-13" };
            MySqlDao mySqlDao = new MySqlDao(TestConnection());
            CommandList expectedComand = CommandList.SHOW_SCHEDULE_FOR_MONTH;

            mySqlDao.CreateQuerry(testCommand);

            Assert.AreEqual(expectedComand, mySqlDao.GetCommand());

        }

        [TestMethod]
        public void TestRecognizeCommandForShowWeekSchedule()
        {
            string[] testCommand = { "ShowScheduleForWeek", "5", "2017-3-13" };
            MySqlDao mySqlDao = new MySqlDao(TestConnection());
            CommandList expectedComand = CommandList.SHOW_SCHEDULE_FOR_WEEK;

            mySqlDao.CreateQuerry(testCommand);

            Assert.AreEqual(expectedComand, mySqlDao.GetCommand());
        }

        [TestMethod]
        public void TestRecognizeCommandForGroupDetail()
        {
            string[] testCommand = { "ShowGroupDetail", "1" };
            MySqlDao mySqlDao = new MySqlDao(TestConnection());
            CommandList expectedComand = CommandList.SHOW_GROUP_DETAIL;

            mySqlDao.CreateQuerry(testCommand);

            Assert.AreEqual(expectedComand, mySqlDao.GetCommand());
        }

        [TestMethod]
        public void TestRecognizeCommandForSubjectDetail()
        {
            string[] testCommand = {"ShowSubjectDetail", "5","5", "2017-03-16 13:20:00", "КМ-135" };
            MySqlDao mySqlDao = new MySqlDao(TestConnection());
            CommandList expectedComand = CommandList.SHOW_SUBJECT_DETAIL;

            mySqlDao.CreateQuerry(testCommand);

            Assert.AreEqual(expectedComand, mySqlDao.GetCommand());
        }

    }
}
