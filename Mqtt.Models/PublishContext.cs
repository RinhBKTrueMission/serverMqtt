using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mqtt.Models
{
    public class PublishContext
    {
        public string Topic { get; set; }
        public object Value { get; set; }
        public byte QOS { get; set; }
        public bool Retain { get; set; }

        public PublishContext() { }
        public PublishContext(string topic, object value)
        {
            this.Topic = topic;
            this.Value = value;
        }
        public PublishContext(string topic, object value, byte qos)
        {
            this.Topic = topic;
            this.Value = value;
            this.QOS = qos;
        }
        public PublishContext(string topic, object value, byte qos, bool retain)
        {
            this.Topic = topic;
            this.Value = value;
            this.QOS = qos;
            this.Retain = retain;
        }
    }
}
