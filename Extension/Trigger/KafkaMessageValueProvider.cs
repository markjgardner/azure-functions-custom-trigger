using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace KafkaExtension.Trigger 
{
  internal class KafkaMessageValueProvider : IValueProvider
  {
    private readonly ConsumeResult<Ignore, string> _message;
    private readonly object _value;
    private readonly Type _valueType;

    public Type Type
    {
      get { return _valueType; }
    }

    public KafkaMessageValueProvider(ConsumeResult<Ignore, string> message, object value, Type valueType)
    {
      if (value != null && !valueType.IsAssignableFrom(value.GetType()))
      {
        throw new InvalidOperationException("value is not of the correct type.");
      }

      _message = message;
      _value = value;
      _valueType = valueType;
    }

    public Task<object> GetValueAsync()
    {
      return Task.FromResult(_value);
    }

    public string ToInvokeString()
    {
      return _message.Message.Value;
    }
  }
}