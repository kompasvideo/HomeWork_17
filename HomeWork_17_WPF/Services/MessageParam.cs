using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork_17_WPF.Services
{
    public class MessageParam
    {
        public DateTime Date { get; set; }
        public MessageType MesType { get; set; }
        public string Text { get; set; }
        public MessageParam()
        {
        }
        public MessageParam(DateTime dateTime, MessageType messageType, string text)
        {
            Date = dateTime;
            MesType = messageType;
            Text = text;
        }
    }
}
