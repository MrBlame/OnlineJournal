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
    class MySqlDao
    {
        private MySqlConnection connection;
        private List<DataTable> tablesToSend;
        private string error;

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
                case "ShowForMonth": { Console.WriteLine("Show for month!"); ShowForMonth(request); break; }
                case "ShowForWeek": { ShowForWeek(request); break; }
                case "ShowForGroup": { ShowForGroup(request); break; }
                case "ShowForSubject": { ShowForSubject(request); break; }
                case "Login": { Console.WriteLine("Login check!"); Login(request); break; }
                default: { WrongCommand(); break; }
            }
        }

        private void Login(string[] request)
        {
            string query = "SELECT ID FROM users" +
                 " WHERE LOGIN = '" + request[1] + "'" +
                 " AND PASSWORD = '" + request[2] + "';";
            tablesToSend.Add(RunQuery(query));
        }

        private void WrongCommand()
        {
            error = "Wrong command syntax!";
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
            //Standart for request: "ShowForSubject;Teacher_Id;Subject_Id;#InDay;Date;Groups"
            //Exemple: ShowForSubject;1;2017-03-16 13:20:00;КМ-135;СП-136
            // Select subject info + all groups on this lesson
            string[] groupNames = SelectGroupNamesFromIncommingRequest(request[4]);
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
                " AND c.DATE = " + date +
                " AND c.TEACHER_ID = b.ID" +
                " AND c.SUBJECT_ID = a.ID";
            tablesToSend.Add(RunQuery(query));
        }

        private string[] SelectGroupNamesFromIncommingRequest(string groupNames)
        {
            string[] result = groupNames.Split(',');
            for (int i = 0; i < result.Count(); i++)
            {
                result[i] = result[i].Trim();
            }
            return result;
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
                "b.PRESENCE FROM students a, studentvisiting b" +
                " WHERE GROUP_ID = " + request[1] +
                " AND a.ID = b.STUDENT_ID ORDER BY ID_IN_GROUP ASC;";
            tablesToSend.Add(RunQuery(query));
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
                error = "No information was found!";
            }
            return table;
        }

        public List<DataTable> GetData()
        {
            return tablesToSend;
        }

        public string GetError()
        {
            return error;
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
}
