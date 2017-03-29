using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
   public enum CommandList
    {
        EMPTY_RESULT,
        WRONG_COMMAND,
        LOGIN,
        INSERT_PRESENCE,
        INSERT_MARK,
        SHOW_SCHEDULE_FOR_WEEK,
        SHOW_SCHEDULE_FOR_MONTH,
        SHOW_SUBJECT_DETAIL,
        SHOW_GROUP_DETAIL,
    }
}
