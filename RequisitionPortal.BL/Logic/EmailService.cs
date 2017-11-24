using RequisitionPortal.BL.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace RequisitionPortal.BL.Logic
{
    public class EmailService:IEmailService
    {
        public string SendEmail(string to, string subject, string message)
        {
            try
            {
                var mail = new Outlook.Application();

                Outlook.MailItem olMailItem = mail.CreateItem(0);//Outlook.OlItemType.olMailItem

                olMailItem.Subject = subject;
                olMailItem.To = to;
                olMailItem.Body = message;

                olMailItem.Send();

                return "sent";
            }
            catch
            {
                return null;
            }

        }
    }
}
