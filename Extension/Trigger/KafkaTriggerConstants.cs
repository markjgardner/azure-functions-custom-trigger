using System;

namespace KafkaExtension.Trigger
{
  internal static class KafkaTriggerConstants 
  {
    public const string TriggerName = "KafkaTrigger";
    public const string TriggerDescription = "New messages received on topic {0} by consumer {1} at {2}";
    public const int MetricWindowSize = 1000;

  }
}