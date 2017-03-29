using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class CommandRecognizer
    {
        private CommandList command;
        // private AbstractClass generalDao;

        // method must return object
        private void CreateDaoObjectBasedOnRequest(string[] request)
        {
            switch (request[0])
            {
                case "ShowScheduleForMonth":
                    {
                        command = CommandList.SHOW_SCHEDULE_FOR_MONTH;
                        // create object on abstract class. Put request to class. Return object.
                        break;
                    }
                case "ShowScheduleForWeek":
                    {
                        command = CommandList.SHOW_SCHEDULE_FOR_WEEK;
                        
                        break;
                    }
                case "ShowGroupDetail":
                    {
                        command = CommandList.SHOW_GROUP_DETAIL;
                        
                        break;
                    }
                case "ShowSubjectDetail":
                    {
                        command = CommandList.SHOW_SUBJECT_DETAIL;
                        
                        break;
                    }
                case "Login":
                    {
                        command = CommandList.LOGIN;
                        
                        break;
                    }
                case "InsertStudentPresence":
                    {
                        command = CommandList.INSERT_PRESENCE;
                        
                        break;
                    }
                case "InsertStudentMark":
                    {
                        command = CommandList.INSERT_MARK;
                        
                        break;
                    }
                default:
                    {
                        command = CommandList.WRONG_COMMAND;
                        break;
                    }
            }
        }
    }
}
