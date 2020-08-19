using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using KafkaExtension.Trigger;

namespace Test
{
    public static class KafkaFunc
    {
        [FunctionName("KafkaFunc")]
        public static void Run([KafkaTrigger("localhost:9092","mygroup","mytopic")]string message, ILogger log)
        {
            Console.WriteLine($"received message: {message}");
        }
    }
}