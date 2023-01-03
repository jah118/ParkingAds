// using System.Text;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.Extensions.Options;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using RedisConsumerPullAds.Facade;
// using RedisConsumerPullAds.util;
//
// namespace RedisConsumerPullAds.RabbitMQ;
//
// public class Consumer
// {
//     private readonly IMemoryCache _memoryCache;
//     private readonly IAppSettings _appSettings;
//
//     ConnectionFactory _factory { get; set; }
//     IConnection _connection { get; set; }
//     IModel _channel { get; set; }
//
//     public Consumer(IMemoryCache memoryCache,IOptionsMonitor<AppSettings> optionsMonitor)
//     {
//         _memoryCache = memoryCache;
//     }
//
//     public void ReceiveMessageFromQ()
//     {
//         try
//         {
//             var factory = new ConnectionFactory {HostName = appSettings.RabbitConn};
//             using var connection = factory.CreateConnection();
//             using var channel = connection.CreateModel();
//             {
//                 _channel.QueueDeclare(queue: "counter",
//                     durable: true,
//                     exclusive: false,
//                     autoDelete: false,
//                     arguments: null);
//
//                 _channel.BasicQos(prefetchSize: 0, prefetchCount: 3, global: false);
//
//                 var consumer = new EventingBasicConsumer(_channel);
//                 consumer.Received += async (model, ea) =>
//                 {
//                     var body = ea.Body;
//                     var message = Encoding.UTF8.GetString(body);
//
//                     Dictionary<string, int> messages = null;
//                     _memoryCache.TryGetValue<Dictionary<string, int>>("messages", out messages);
//                     if (messages == null) messages = new Dictionary<string, int>();
//
//                     Console.WriteLine(" [x] Received {0}", message);
//                     Thread.Sleep(3000);
//
//                     messages.Remove(message);
//                     _memoryCache.Set<Dictionary<string, int>>("messages", messages);
//                     _channel.BasicAck(ea.DeliveryTag, false);
//                 };
//
//                 _channel.BasicConsume(queue: "counter",
//                     autoAck: false,
//                     consumer: consumer);
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"{ex.Message} | {ex.StackTrace}");
//         }
//     }
// }