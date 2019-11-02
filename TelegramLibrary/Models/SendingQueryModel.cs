using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLibrary.Models
{
    public class SendingQueryModel
    {
        public string SendingChannel { get; set; }
        public string PhotoPath { get; set; }
        public string SendingText { get; set; }
    }
}
