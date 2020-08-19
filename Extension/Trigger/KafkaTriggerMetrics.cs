using System;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace KafkaExtension.Trigger
{
  internal class KafkaTriggerMetrics : ScaleMetrics
  {
    public int ConsumedMessagesInWindow { get; set; }
  }
}