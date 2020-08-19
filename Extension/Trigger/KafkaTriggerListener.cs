using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Scale;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Host.Executors;

namespace KafkaExtension.Trigger
{
  internal class KafkaTriggerListener : IListener, IScaleMonitor<KafkaTriggerMetrics>
  {
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private IConsumer<Ignore, string> _consumer;
    private readonly ILogger _logger;
    private string _functionId;
    private ScaleMonitorDescriptor _scaleMonitorDescriptor;
    private bool _isDisposed;
    private bool _isStopping;
    private KafkaTriggerMetrics _metric;
    private ITriggeredFunctionExecutor _executor;

    public KafkaTriggerListener(ITriggeredFunctionExecutor executor, string functionId, 
            ConsumerConfig kafkaConfig, string TopicName, ILogger logger) 
    {
      this._executor = executor;
      this._functionId = functionId;
      this._logger = logger;
      this._config = kafkaConfig;
      this._topic = TopicName;
      this._consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
      this._metric = new KafkaTriggerMetrics() { ConsumedMessagesInWindow=0 };
      this._scaleMonitorDescriptor = new ScaleMonitorDescriptor($"{_functionId}-KafkaTrigger-{_topic}-{_config.GroupId}".ToLower());
    }

    public ScaleMonitorDescriptor Descriptor
    {
      get
      {
        return _scaleMonitorDescriptor;
      }
    }

    public void Cancel()
    {
      this.StopAsync(CancellationToken.None).Wait();
    }

    public void Dispose()
    {
      if (!_isDisposed)
      {
        if (_consumer != null)
        {
          _consumer.Dispose();
          _consumer = null;
        }

        _isDisposed = true;
      }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _consumer.Subscribe(_topic);
      while (!_isStopping)
      {
        var message = await Task.Run(() => _consumer.Consume(cancellationToken));
        _metric.ConsumedMessagesInWindow++;
        var triggerData = new TriggeredFunctionData
        {
          TriggerValue = message
        };
        await _executor.TryExecuteAsync(triggerData, CancellationToken.None);
      }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _isStopping = true;
      await Task.Run(() => _consumer.Unsubscribe());
      await Task.Run(() => _consumer.Close());
    }

    int CountMessagesReceived()
    {
      this._metric.ConsumedMessagesInWindow=0;
      Thread.Sleep(KafkaTriggerConstants.MetricWindowSize);
      return this._metric.ConsumedMessagesInWindow;
    }

    async Task<ScaleMetrics> IScaleMonitor.GetMetricsAsync()
    {
      return await GetMetricsAsync();
    }

    public async Task<KafkaTriggerMetrics> GetMetricsAsync()
    {
      var messages = await Task.Run(()=>CountMessagesReceived());
      return new KafkaTriggerMetrics() { ConsumedMessagesInWindow=messages };      
    }

    public ScaleStatus GetScaleStatus(ScaleStatusContext<KafkaTriggerMetrics> context)
    {
      var status = new ScaleStatus();
      if(context.WorkerCount < context.Metrics.FirstOrDefault().ConsumedMessagesInWindow/1000)
      {
        status.Vote = ScaleVote.ScaleOut;
      }
      else if(context.WorkerCount > context.Metrics.FirstOrDefault().ConsumedMessagesInWindow/1000)
      {
        status.Vote = ScaleVote.ScaleIn;
      }
      else
      {
        status.Vote = ScaleVote.None;
      }

      return status;
    }

    public ScaleStatus GetScaleStatus(ScaleStatusContext context)
    {
      return GetScaleStatus((ScaleStatusContext<KafkaTriggerMetrics>)context);
    }
  }
}