using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagerRW4
{
    interface IServiceThread
    {

        Task InvokeThread(long lSysServiceThread_Id);

        long lThreadId
        {
            get;
        }

        string sThreadStatus
        {
            get;
            set;
        }

        string sRequestedStatus
        {
            get;
            set;
        }
    }
}
