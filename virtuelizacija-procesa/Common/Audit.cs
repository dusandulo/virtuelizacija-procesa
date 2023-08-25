using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }

        public Audit()
        {

        }
        public Audit(int id, DateTime timeStamp, MessageType messageType, string message)
        {
            Id = id;
            TimeStamp = timeStamp;
            MessageType = messageType;
            Message = message;
        }

    }
}
