using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace Server
{
   public class MySqlDao
    {
        private MySqlConnection connection;
        private List<DataTable> tablesToSend;
        private CommandList command;

        public MySqlDao(MySqlConnection connection)
        {
            this.connection = connection;
            tablesToSend = new List<DataTable>();
        }

        public void CreateQuerry(string[] request)
        {
            tablesToSend.Clear();
            RecognizeCommand(request);
        }

        private void RecognizeCommand(string[] request)
        {
            switch (request[0])
            {
                case "ShowScheduleForMonth":
                    {
                        command = CommandList.SHOW_SCHEDULE_FOR_MONTH;
                        ShowForMonth(request);
                        break;
                    }
                case "ShowScheduleForWeek":
                    {
                        command = CommandList.SHOW_SCHEDULE_FOR_WEEK;
                        ShowForWeek(request);
                        break;
                    }
                case "ShowGroupDetail":
                    {
                        command = CommandList.SHOW_GROUP_DETAIL;
                        ShowForGroup(request);
                        break;
                    }
                case "ShowSubjectDetail":
                    {
                        command = CommandList.SHOW_SUBJECT_DETAIL;
                        ShowForSubject(request);
                        break;
                    }
                case "Login":
                    {
                        command = CommandList.LOGIN;
                        Login(request);
                        break;
                    }
                case "InsertStudentPresence":
                    {
                        command = CommandList.INSERT_PRESENCE;
                        InsertStudentsPresence(request);
                        break;
                    }
                case "InsertStudentMark":
                    {
                        command = CommandList.INSERT_MARK;
                        InsertStudentsMark(request);
                        break;
                    }
                default:
                    {
                        command = CommandList.WRONG_COMMAND;
                        break;
                    }
            }
        }

        private void Login(string[] request)
        {
            string query = "SELECT ID FROM users" +
                 " WHERE LOGIN = '" + request[1] + "'" +
                 " AND PASSWORD = '" + request[2] + "';";
            tablesToSend.Add(RunQuery(query));
        }

        private void ShowForWeek(string[] request)
        {
            DateTime date = DateTime.Parse(request[2]);
            Console.WriteLine((int)date.DayOfWeek);
            string query = "SELECT * FROM schedule" +
                " WHERE week(DATE) = " + GetWeekOfYear(date) +
                " AND year(DATE) = " + date.Year +
                " AND TEACHER_ID = " + Int32.Parse(request[1]) + ";";
            tablesToSend.Add(RunQuery(query));
        }

        private int GetWeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void ShowForMonth(string[] request)
        {
            DateTime date = DateTime.Parse(request[2]);
            string query = "SELECT * FROM schedule" +
                " WHERE month(DATE) = " + date.Month +
                " AND year(DATE) = " + date.Year +
                " AND TEACHER_ID = " + Int32.Parse(request[1]) + ";";
            tablesToSend.Add(RunQuery(query));
        }

        private void ShowForSubject(string[] request)
        {
            //Standart for request: "ShowForSubject;Teacher_Id;Subject_Id;Date;Groups"
            //Exemple: ShowForSubject;1;5; 2017-03-16 13:20:00;КМ-135;СП-136
            // Select subject info + all groups on this lesson
            string[] groupNames = SelectObjectsListFromRequest(request[4]);
            string teacherId = request[1];
            string subjectId = request[2];
            string date = request[3];


            SelectSubjectInfo(teacherId, subjectId, date);
            GenerateQueryForGroupSelecting(groupNames, subjectId, date);
        }

        private void SelectSubjectInfo(string teacherId, string subjectId, string date)
        {
            string query = "SELECT a.NAME,b.NAME,b.SURNAME,b.POSITION,c.CLASSROOM," +
                "CONVERT(c.DATE USING utf8),c.SUBJECT_TYPE,c.THEME_OF_LESSON,c.CONDUCTED" +
                " FROM subjects a, teachers b, schedule c" +
                " WHERE c.TEACHER_ID = " + teacherId +
                " AND c.SUBJECT_ID = " + subjectId +
                " AND c.DATE = '" + date +"'"+
                " AND c.TEACHER_ID = b.ID" +
                " AND c.SUBJECT_ID = a.ID";
            tablesToSend.Add(RunQuery(query));
        }

        private void GenerateQueryForGroupSelecting(string[] groupNames, string subjectId, string date)
        {
            List<DataTable> groups = new List<DataTable>();

            for (int i = 0; i < groupNames.Count(); i++)
            {
                List<string> requestForGroup = new List<string>();
                requestForGroup.Add("ShowForGroup");
                requestForGroup.Add(SelectGroupIdByGroupName(groupNames[i]));
                requestForGroup.Add(subjectId);
                requestForGroup.Add(date);
                ShowForGroup(requestForGroup.ToArray());
            }
        }

        private string SelectGroupIdByGroupName(string groupName)
        {
            string query = "SELECT ID FROM groups" +
                " WHERE NAME = '" + groupName + "';";
            int result = 0;

            MySqlCommand sqlCom = new MySqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            MySqlDataReader dataReader = sqlCom.ExecuteReader();
            while (dataReader.Read())
            {
                result = (Int32)dataReader["ID"];
            }
            dataReader.Close();
            return result.ToString();
        }

        private void ShowForGroup(string[] request)
        {
            string query = "SELECT a.ID_IN_GROUP,a.SURNAME,a.NAME," +
                "b.PRESENCE FROM students a, studentacademicperformance b" +
                " WHERE GROUP_ID = " + request[1] +
                " AND a.ID = b.STUDENT_ID ORDER BY ID_IN_GROUP ASC;";
            tablesToSend.Add(RunQuery(query));
        }

        private void InsertStudentsPresence(string[] request)
        {
            // Standart template: "InserrtStudentPresence;Subject_Id;Date;List<StudentId>;List<Value>"
            string[] studentList = SelectObjectsListFromRequest(request[3]);
            string[] valueList = SelectObjectsListFromRequest(request[4]);
            for (int i = 0; i < studentList.Count(); i++)
            {
                string query = "UPDATE studentacademicperformance" +
                    " SET PRESENCE = " + valueList[i] +
                    " WHERE STUDENT_ID = " + studentList[i] +
                    " AND SUBJECT_ID = " + request[1] +
                    " AND DATE = '" + request[2] + "';";
                Console.WriteLine("Message for student id[" + studentList[i] + "] was send");
                RunQuery(query);
            }
        }

        private void InsertStudentsMark(string[] request)
        {
            // Standart template: "InsertStudentMark;Subject_Id;Date;List<StudentId>;List<Value>; List<Mar_Types>; List<Description>"
            string[] studentList = SelectObjectsListFromRequest(request[3]);
            string[] valueList = SelectObjectsListFromRequest(request[4]);
            string[] markTypesList = SelectObjectsListFromRequest(request[5]);
            string[] descriptionList = SelectObjectsListFromRequest(request[6]);
            for (int i = 0; i < studentList.Count(); i++)
            {
                string query = "UPDATE studentacademicperformance" +
                    " SET MARK = " + valueList[i] + "," +
                    "MARK_TYPE = " + markTypesList[i] + "," +
                    "MARK_DESCRIPTION = '" + descriptionList[i] + "'" +
                    " WHERE STUDENT_ID = " + studentList[i] +
                    " AND SUBJECT_ID = " + request[1] +
                    " AND DATE = '" + request[2] + "';";
                Console.WriteLine("Message for student id[" + studentList[i] + "] was send");
                RunQuery(query);
            }
        }

        private string[] SelectObjectsListFromRequest(string request)
        {
            string[] result = request.Split(',');
            for (int i = 0; i < result.Count(); i++)
            {
                result[i] = result[i].Trim();
            }
            return result;
        }

        private DataTable RunQuery(string query)
        {
            DataTable table = new DataTable();
            MySqlCommand sqlCom = new MySqlCommand(query, connection);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            sqlCom.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            dataAdapter.Fill(table);
            if (table.Rows.Count == 0)
            {
                command = CommandList.EMPTY_RESULT;
            }
            return table;
        }

        public List<DataTable> GetData()
        {
            return tablesToSend;
        }

        public CommandList GetCommand()
        {
            return command;
        }

        public void CloseConnection()
        {
            connection.Close();
        }

    }
}
