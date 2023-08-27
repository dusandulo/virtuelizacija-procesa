using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Audit
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public MessageType MessageType { get; set; }
        [DataMember]
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
