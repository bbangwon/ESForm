using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESForm
{
    public class ESLogBase
    {
        public static Action<string> onLog;

        public static void Log(string logmsg)
        {
            if (onLog != null)
                onLog(logmsg);
        }
    }
}
