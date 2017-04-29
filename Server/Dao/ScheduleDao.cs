using FastMember;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Dao
{
    public class ScheduleDao : GeneralDao
    {
        private int teacherId;
        private string begDate;
        private string endDate;
        public ScheduleDao()
        {
            tablesToSend = new List<DataTable>();
        }

        public void ShowForPeriod(string[] request)
        {
            // ShowSchedule;1;2017-03-01;2017-03-31
            SetData(request);
            SetQuery();
            AddTableToResult();
            WriteDataToFile(tablesToSend);
        }

        private void SetData(string[] request)
        {
            teacherId = Int32.Parse(request[1]);
            begDate = DateTime.Parse(request[2]).ToString("yyyy-MM-dd");
            endDate = DateTime.Parse(request[3]).ToString("yyyy-MM-dd");
        }

        private void SetQuery()
        {
            query =
               "SELECT c.ID,c.SUBJECT_ID,a.NAME,a.SHORT_NAME," +
               "b.SURNAME,b.FIRST_NAME,b.MIDDLE_NAME," +
               "c.CLASSROOM,c.GROUPS," +
               "CONVERT(c.LESSON_DATE USING utf8)," +
               "c.SUBJECT_TYPE,c.THEME_OF_LESSON,c.CONDUCTED,c.FILES_FOR_LESSON" +
               " FROM subjects a, teachers b, schedule c" +
               " WHERE TEACHER_ID = " + teacherId +
               " AND LESSON_DATE BETWEEN '" + begDate + "' AND '" + endDate + "'" +
               " AND c.SUBJECT_ID = a.ID" +
               " AND c.TEACHER_ID = b.USER_ID" +
               " COLLATE cp1251_ukrainian_ci";
        }

        public void ShowForGroup(string[] request)
        {
            // ShowScheduleForGroup;5;2017-03-01;2017-03-31
            ShowForPeriod(request);

            List<string> groups = GetAllGroupsForSort();
            List<DataTable> result = new List<DataTable>();

            foreach (var k in groups)
            {
                IEnumerable<DataRow> query = from dr in tablesToSend[0].AsEnumerable()
                                             where dr.Field<string>("GROUPS").Contains(k)
                                             select dr;
                DataTable dt = query.CopyToDataTable<DataRow>();
                result.Add(dt);
            }

            tablesToSend.Clear();
            tablesToSend.AddRange(result);

            WriteDataToFile(tablesToSend);
        }

        private List<string> GetAllGroupsForSort()
        {
            List<string> groups = tablesToSend[0].AsEnumerable()
                .Select(g => g.Field<string>("GROUPS")).Distinct().ToList();
            return SortGroups(groups);
        }

        private List<string> SortGroups(List<string> groupsList)
        {
            List<string> result = new List<string>();
            foreach (var k in groupsList)
            {
                result.AddRange(k.Split(','));
            }

            for (int i = 0; i < result.Count; i++)
            {
                result[i] = result[i].Trim();
            }

            return result.Distinct().ToList();
        }

        public void ShowForSubject(string[] request)
        {
            ShowForPeriod(request);

            List<int> groups = GetAllSubjectsForSort();
            List<DataTable> result = new List<DataTable>();

            foreach (var k in groups)
            {
                IEnumerable<DataRow> query = from dr in tablesToSend[0].AsEnumerable()
                                             where dr.Field<int>("SUBJECT_ID").Equals(k)
                                             select dr;
                DataTable dt = query.CopyToDataTable<DataRow>();
                result.Add(dt);
            }

            tablesToSend.Clear();
            tablesToSend.AddRange(result);

            WriteDataToFile(tablesToSend);
        }

        private List<int> GetAllSubjectsForSort()
        {
            List<int> subjects = tablesToSend[0].AsEnumerable()
                .Select(g => g.Field<int>("SUBJECT_ID")).Distinct().ToList();
            return subjects;
        }

        //private int GetWeekOfYear(DateTime time)
        //{
        //    DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
        //    if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        //    {
        //        time = time.AddDays(3);
        //    }
        //    return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        //}

    }


}
