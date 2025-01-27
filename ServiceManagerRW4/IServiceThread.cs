using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManagerRW4
{
    public interface IServiceManager
    {
        Task InvokeServiceAsync(CancellationToken cancellationToken);
        public bool LoadThreads();
    }

}
