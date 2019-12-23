using System;
using System.Net;
using System.Net.Mail;

namespace EffortEstimator.Helpers
{
    public class MailOperator
    {
        private readonly NetworkCredential credentials;
        public MailOperator()
        {
            credentials = new NetworkCredential(Properties.Resources.AppMailCredentialsEmail, Properties.Resources.AppMailCredentialsPassword);
        }

        public void SendUserActivationKey(string userEmailAddress, string key)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress("noreply@EffortEstimator.com"),
                    Subject = "Account activation key",
                    Body = key
                };

                mail.To.Add(userEmailAddress);

                SmtpClient client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
            }
            catch(Exception e)
            {
                throw new Exception("Failed to send User Activaction Key  " + e.Message);
            }

        }

        public void SendUserConferenceInfo(string userEmailAddress, string groupName, string topic, DateTime startDate)
        {
            try
            {
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress("noreply@EffortEstimator.com"),
                    Subject = "New conference has been scheduled",
                    Body = "Day " + startDate.ToString("MM/dd/yyyy") + " at " + startDate.ToString("hh:mm tt") + " in group [" + groupName + "] will start a conference, which topic being [" + topic + "]"
                };

                mail.To.Add(userEmailAddress);

                SmtpClient client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };
                client.Send(mail);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to send User Activaction Key  " + e.Message);
            }
        }


        public static bool IsValidEmail(string email)
        {
            try
            {
                MailAddress addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
