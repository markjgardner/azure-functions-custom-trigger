using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Confluent.Kafka;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace KafkaExtension.Trigger
{
  internal class KafkaTriggerBinding : ITriggerBinding
  {
    private readonly ParameterInfo _parameter;
    private readonly ILogger _logger;
    private readonly ConsumerConfig _kafkaConfig;
    private readonly string _topicName;
    private readonly Dictionary<string, Type> _bindingContract;

    public Type TriggerValueType => typeof(ConsumeResult<Ignore,string>);

    public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;
    

    public KafkaTriggerBinding(ParameterInfo parameter, ConsumerConfig kafkaConfig, String topicName, ILogger logger)
    {
      _parameter = parameter;
      _kafkaConfig = kafkaConfig;
      _topicName = topicName;
      _logger = logger;
      _bindingContract = new Dictionary<string, Type>();
      _bindingContract.Add("message", typeof(string));
    }

    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
      ConsumeResult<Ignore, string> message = value as ConsumeResult<Ignore, string>;
      if (message != null)
      {
        var bindingData = new Dictionary<string, object>();
        bindingData.Add("message", message.Message.Value);

        var valueProvider = new KafkaMessageValueProvider(message, message.Message.Value, typeof(string));

        return Task.FromResult<ITriggerData>(new TriggerData(valueProvider,bindingData));
      }
      throw new ArgumentException("Cannot map received message to parameter type");
    }

    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context", "Missing listener context");
      }

      return Task.FromResult<IListener>(new KafkaTriggerListener(
          context.Executor,
          context.Descriptor.Id,
          this._kafkaConfig,
          this._topicName,
          this._logger));
    }

    public ParameterDescriptor ToParameterDescriptor()
    {
      return new KafkaTriggerParameterDescriptor
      {
        Name = _parameter.Name,
        Type = KafkaTriggerConstants.TriggerName,
        TopicName = this._topicName,
        GroupId = this._kafkaConfig.GroupId,
      };
    }
  }
}