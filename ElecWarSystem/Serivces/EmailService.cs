using ElecWarSystem.Data;
using ElecWarSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElecWarSystem.Serivces
{
    public class EmailService
    {
        private AppDBContext dBContext;
        public EmailService()
        {
            dBContext = new AppDBContext();
        }
        public List<Email> GetExportedEmails(int unitID)
        {
            List<Email> emails = dBContext.Emails
                .Include("Sender")
                .Where(row => row.SenderUserID == unitID)
                .OrderByDescending(m => m.SendDateTime)
                .ToList();
            return emails;
        }
        public List<Email> GetRecievedEmails(int unitID)
        {
            List<Email> emails = dBContext.Emails
                .Include("Sender")
                .Include("Recievers")
                .Where(row => row.Recievers.Contains(row.Recievers.FirstOrDefault(rec => rec.RecieverID == unitID)))
                .OrderByDescending(m => m.SendDateTime)
                .ToList();
            return emails;
        }
        public List<Reciever> GetRecievers(int unitID)
        {
            List<Reciever> recievers = dBContext.Recievers
                .Include("Email.Sender")
                .Where(row => row.RecieverID == unitID)
                .OrderByDescending(m => m.Email.SendDateTime)
                .ToList();
            foreach (Reciever reciever in recievers)
            {
                reciever.Email.Recievers = null;
            }
            return recievers;
        }
        public int GetCountOfUnReadEmails(int unitID)
        {
            int count = dBContext.Recievers.Where(row => row.RecieverID == unitID && !row.Readed).Count(); ;
            return count;
        }
    }
}