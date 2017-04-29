using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Dao;
using System.Globalization;

namespace Server
{
    class CommandRecognizer
    {
        private MySqlConnection connection { get; set; }
        private List<DataTable> tablesToSend { get; set; }
        private ServerStatus serverStatus { get; set; }

        public CommandRecognizer(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void GenerateDataFromRequest(string[] request)
        {
            tablesToSend = new List<DataTable>();
            switch (request[0])
            {
                case "Login":
                    {
                        Login(request);
                        break;
                    }
                case "ShowSchedule":
                    {
                        ShowSchedule(request);
                        break;
                    }

                case "ShowScheduleForGroup":
                    {
                        ShowScheduleForGroup(request);
                        break;
                    }
                case "ShowScheduleForSubject":
                    {
                        ShowScheduleForSubject(request);
                        break;
                    }
                case "ShowSubjectDetail":
                    {
                        SubjectDetails(request);
                        break;
                    }
                case "InsertStudentPerformance":
                    {
                        InsertStudentPerformance(request);
                        break;
                    }
                default:
                    {
                        serverStatus = ServerStatus.WRONG_COMMAND;
                        break;
                    }
            }
        }

        private void Login(string[] request)
        {
            serverStatus = ServerStatus.LOGIN;
            UserDao userDao = new UserDao();
            userDao.SetConnection(connection);
            userDao.SetCommand(serverStatus);
            userDao.Login(request);
            tablesToSend.AddRange(userDao.GetData());
        }

        private void ShowSchedule(string[] request)
        {
            serverStatus = ServerStatus.SHOW_SCHEDULE;
            ScheduleDao scheduleDao = new ScheduleDao();
            scheduleDao.SetConnection(connection);
            scheduleDao.SetCommand(serverStatus);
            scheduleDao.ShowForPeriod(request);
            tablesToSend.AddRange(scheduleDao.GetData());
        }

        private void ShowScheduleForGroup(string[] request)
        {
            serverStatus = ServerStatus.SHOW_SCHEDULE_FOR_GROUPS;
            ScheduleDao scheduleDao = new ScheduleDao();
            scheduleDao.SetConnection(connection);
            scheduleDao.SetCommand(serverStatus);
            scheduleDao.ShowForGroup(request);
            tablesToSend.AddRange(scheduleDao.GetData());
        }

        private void ShowScheduleForSubject(string[] request)
        {
            serverStatus = ServerStatus.SHOW_SCHEDULE_FOR_SUBJECTS;
            ScheduleDao scheduleDao = new ScheduleDao();
            scheduleDao.SetConnection(connection);
            scheduleDao.SetCommand(serverStatus);
            scheduleDao.ShowForSubject(request);
            tablesToSend.AddRange(scheduleDao.GetData());
        }

        private void SubjectDetails(string[] request)
        {
            serverStatus = ServerStatus.SHOW_SUBJECT_DETAIL;
            SubjectDao subjectDao = new SubjectDao();
            subjectDao.SetConnection(connection);
            subjectDao.SetCommand(serverStatus);
            subjectDao.ShowForSubject(request);
            tablesToSend.AddRange(subjectDao.GetData());
        }

        private void InsertStudentPerformance(string[] request)
        {
            serverStatus = ServerStatus.INSERT_PERFORMANCE;
            StudentPerformanceDao spDao = new StudentPerformanceDao();
            spDao.SetConnection(connection);
            spDao.SetCommand(serverStatus);
            spDao.InsertPerformance(request);
            tablesToSend.AddRange(spDao.GetData());
        }

        public List<DataTable> GetTables()
        {
            return tablesToSend;
        }

        public ServerStatus GetCommand()
        {
            return serverStatus;
        }

    }
}
