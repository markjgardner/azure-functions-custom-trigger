
using KafkaExtension;
using KafkaExtension.Trigger;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

[assembly: WebJobsStartup(typeof(KafkaWebJobsStartup))]

namespace KafkaExtension
{
    public class KafkaWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            
            builder.Services.TryAddSingleton<KafkaTriggerAttributeBindingProvider>();
            builder.AddExtension<KafkaExtensionConfigProvider>();
        }
    }
}
