using System.Text;
using CodeHub.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMqProducerService
{
    public static event Action<string>? ResultReceived;
    public async Task SendToRabbitMq(string code, string language)
    {
        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@host.docker.internal:5672") };
        factory.ClientProvidedName = "RabbitMqProducer";

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "codeQueue", durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        var message = new CodeExecutionRequest
        {
            Guid = Guid.NewGuid().ToString(),
            SourceCode = code,
            Language = language
        };

        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "codeQueue", mandatory: true, basicProperties: properties, body: body);
    }

    public async Task ListenForResults()
    {
        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@host.docker.internal:5672") };
        factory.ClientProvidedName = "RabbitMqConsumer";

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "resultsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var result = JsonConvert.DeserializeObject<CodeExecutionResult>(message);

            if (result != null)
            {
                ResultReceived?.Invoke(result.Output);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            else
            {
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync("resultsQueue", autoAck: false, consumer: consumer);
        await Task.Delay(Timeout.Infinite);
    }
}

public class CodeExecutionRequest
{
    public string Guid { get; set; }
    public string SourceCode { get; set; }
    public string Language { get; set; }
}

public class CodeExecutionResult
{
    public string Guid { get; set; }
    public string Output { get; set; }
}