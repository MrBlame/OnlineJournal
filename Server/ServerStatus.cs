using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
   public enum ServerStatus
    {
        EMPTY_RESULT,
        WRONG_COMMAND,
        DB_ERROR_IN_DATA,
        DB_SOME_DATA_NOT_EXIST,
        DB_CORRECT_DATA,
        LOGIN,
        LOGIN_FAILED,
        LOGIN_SUCCESSFULL,
        INSERT_PERFORMANCE,
        INSERT_PRESENCE,
        INSERT_PRESENCE_FAILED,
        INSERT_PRESENCE_SUCCESFULL,
        INSERT_MARK,
        INSERT_MARK_FAILED,
        INSERT_MARK_SUCCESFULL,
        SHOW_SCHEDULE,
        SHOW_SCHEDULE_FOR_GROUPS,
        SHOW_SCHEDULE_FOR_SUBJECTS,
        SHOW_SUBJECT_DETAIL,
        SHOW_GROUP_DETAIL,
    }
}
