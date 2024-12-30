using ElecWarSystem.Models;
using ElecWarSystem.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Description;

namespace ElecWarSystem.ViewModel
{
    public class Reciever
    {
        public int RecId { get; set; }
        public String UnitName { get; set; }
        public bool Checked { get; set; } = false;
    }
    public class EmailViewModel
    {
        public Email Email { get; set; }
        public List<String> RecIds { get; set; }
        public IEnumerable<Reciever> Recievers { get; set; }
        public String Message { get; set; } = "";
        public EmailViewModel()
        {
            UserService userService = new UserService();
            Recievers = userService.getAllUsers().Select(m => new Reciever()
            {
                UnitName = m.UnitName,
                RecId = m.ID
            });
        }
    }
}