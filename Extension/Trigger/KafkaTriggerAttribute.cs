using System;
using Microsoft.Azure.WebJobs.Description;
using Confluent.Kafka;

namespace KafkaExtension.Trigger
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class KafkaTriggerAttribute : Attribute
    {
        public string BootstrapServers { get; private set; }
        public string GroupId { get; private set; }
        public string TopicName { get; private set; }
        public AutoOffsetReset StartOffset { get; private set; }


        public KafkaTriggerAttribute(string BootstrapServers, string GroupId, string TopicName,
                            AutoOffsetReset StartOffset = AutoOffsetReset.Earliest) 
        {
            this.BootstrapServers=BootstrapServers;
            this.GroupId = GroupId;
            this.TopicName = TopicName;
            this.StartOffset = StartOffset;
        }

    }
}
