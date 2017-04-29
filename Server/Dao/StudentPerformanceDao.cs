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
    class StudentPerformanceDao : GeneralDao
    {
        private string subjectId;
        private string lessonDate;
        private string[] studentList;
        private string[] presenceValueList;
        private string[] markValueList;
        private string[] markTypesList;
        private string[] descriptionList;

        public StudentPerformanceDao()
        {
            tablesToSend = new List<DataTable>();
        }

        public void InsertPerformance(string[] request)
        {
            // Standart template: "InsertStudentPresence;Subject_Id;Date;List<StudentId>;List<PresenceValue>;List<MarkValue>; List<Mark_Types>; List<Description>"
            // InsertStudentPerformance;5;2017-03-23 13:20:00;7,8;0,0;80,80;1,1;Lab 2, Lab 2

            SetData(request);
            InsertData();

        }

        private void SetData(string[] request)
        {
            subjectId = request[1];
            lessonDate = request[2];
            studentList = SelectObjectsListFromRequest(request[3]);
            presenceValueList = SelectObjectsListFromRequest(request[4]);
            markValueList = SelectObjectsListFromRequest(request[5]);
            markTypesList = SelectObjectsListFromRequest(request[6]);
            descriptionList = SelectObjectsListFromRequest(request[7]);
        }

        private void InsertData()
        {
            for (int i = 0; i < studentList.Count(); i++)
            {
                query = "UPDATE studentacademicperformance" +
                    " SET PRESENCE = " + presenceValueList[i] + "," +
                    "MARK = " + markValueList[i] + "," +
                    "MARK_TYPE = " + markTypesList[i] + "," +
                    "MARK_DESCRIPTION = '" + descriptionList[i] + "'" +
                    " WHERE STUDENT_ID = " + studentList[i] +
                    " AND SUBJECT_ID = " + subjectId +
                    " AND LESSON_DATE = '" + lessonDate + "';";
                InsertDataInDataBase();
            }
        }

    }
}
