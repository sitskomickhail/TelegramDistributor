using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramTimer.Model
{
    public class DataGridQueryModel
    {
        public DateTime DateTimeSending { get; set; }
        public string SendingChannel { get; set; }
        public string PhotoPath { get; set; }
        public string SendingText { get; set; }
        public string Status { get; set; }
    }
}
