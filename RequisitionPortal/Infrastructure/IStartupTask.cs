using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.Infrastructure
{
    public interface IStartupTask
    {
        void Execute();
        int Order();
    }
}
