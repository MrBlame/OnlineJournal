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
        private string query;

        public MySqlDao(MySqlConnection connection)
        {
            this.connection = connection;

        }

        public DataTable RunQuery(string query)
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
            return table;
        }

        public void CreateQuerry(string[] request)
        {
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

        // All Show* must run querry and return List<DataTable> to gloval variable


        private void Login(string[] request)
        {
            query = "SELECT ID FROM users" +
                " WHERE LOGIN = '" + request[1] + "'" +
                " AND PASSWORD = '" + request[2] + "';";
            Console.WriteLine("Query: " + query);
        }

        private void WrongCommand()
        {
            throw new NotImplementedException();
        }

        private void ShowForSubject(string[] request)
        {
            //Standart for request: "ShowForSubject;Subject_Id;Date;Groups"
            //Exemple: ShowForSubject;1;2017-03-16 13:20:00;КМ-135;СП-136
            // Select subject info + all groups on this lesson
            string[] requestForGroups = ConcatArray(request, 3, request.Count());
            string date = request[2];
            string subjectId = request[1];
            SelectForGroups(requestForGroups,subjectId,date);


            Console.WriteLine("AfterParse:");

            foreach (var k in requestForGroups)
            {
                Console.WriteLine(k);
            }


            //DateTime date = DateTime.Parse(request[2]);
            //Console.WriteLine((int)date.DayOfWeek);
            //query = "SELECT * FROM schedule" +
            //    " WHERE DATE = " + date +
            //    " AND year(DATE) = " + date.Year +
            //    " AND TEACHER_ID = " + Int32.Parse(request[1]) + ";";
            //Console.WriteLine("Query: " + query);
        }

        private string[] ConcatArray(string[] array, int beg, int end)
        {
            List<string> result = new List<string>();
            for (int i = beg; i < end; i++)
            {
                result.Add(array[i]);
            }
            return result.ToArray();
        }

        private void SelectForGroups(string[] requestForGroups,string subjectId, string date)
        {
            List<DataTable> groups = new List<DataTable>();

            for(int i = 0; i < requestForGroups.Count(); i++)
            {
                List<string> requestForGroup = new List<string>();
                requestForGroup.Add("ShowForGroup");
                requestForGroup.Add(requestForGroups[i]);
                requestForGroup.Add(subjectId);
                requestForGroup.Add(date);

            }
        }

        private void ShowForGroup(string[] request)
        {
            // Request template: "ShowForGroup;Group_Id;Subject_Id;Date"
        }

        private void ShowForWeek(string[] request)
        {
            DateTime date = DateTime.Parse(request[2]);
            Console.WriteLine((int)date.DayOfWeek);
            query = "SELECT * FROM schedule" +
                " WHERE week(DATE) = " + GetWeekOfYear(date) +
                " AND year(DATE) = " + date.Year +
                " AND TEACHER_ID = " + Int32.Parse(request[1]) + ";";
            Console.WriteLine("Query: " + query);
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
            query = "SELECT * FROM schedule" +
                " WHERE month(DATE) = " + date.Month +
                " AND year(DATE) = " + date.Year +
                " AND TEACHER_ID = " + Int32.Parse(request[1]) + ";";
            Console.WriteLine("Query: " + query);
        }

        public string GetQuerry()
        {
            return query;
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
}
