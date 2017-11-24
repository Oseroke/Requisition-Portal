using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionPortal.BL.Abstracts
{
    public interface IEmailService
    {
        string SendEmail(string to, string subject, string message);
    }
}
