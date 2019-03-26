using RequisitionPortal.BL.Abstracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Outlook = Microsoft.Office.Interop.Outlook;

namespace RequisitionPortal.BL.Logic
{
    public class EmailService:IEmailService
    {
        public string SendEmail(string to, string cc, string bcc, string subject, string message)
        {
            try
            {
                string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["RequisitionPortalConnectionSetting"].ConnectionString;

                SqlConnection cnn = new SqlConnection(cnnString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "spSendMail";
                cmd.Parameters.Add(new SqlParameter("mailTo", to));
                cmd.Parameters.Add(new SqlParameter("mailCC", cc));
                cmd.Parameters.Add(new SqlParameter("mailBCC", bcc));
                cmd.Parameters.Add(new SqlParameter("mailSubject", subject));
                cmd.Parameters.Add(new SqlParameter("mailbody", message));
                //add any parameters the stored procedure might require
                cnn.Open();
                object o = cmd.ExecuteScalar();
                cnn.Close();

                return "sent";
            }
            catch (Exception ex)
            {
                //string s = ex.Message;
                return null;
            }

        }
    }
}
