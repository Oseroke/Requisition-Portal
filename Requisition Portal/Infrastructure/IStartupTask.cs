using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requisition_Portal.Infrastructure
{
    public interface IStartupTask
    {
        void Execute();
        int Order();
    }
}
