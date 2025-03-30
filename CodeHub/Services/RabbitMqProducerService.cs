using CodeHub.Data.Entities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMqProducerService
{
    public static event Action<string, bool>? ResultReceived;

    public async Task SendToRabbitMq(string code, string language, List<TestCase> testCases, bool isEvaluation)
    {
        var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@host.docker.internal:5672") };
        factory.ClientProvidedName = "RabbitMqProducer";

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        string queueName = language switch
        {
            "Java" => "javaQueue",
            "C#" => "csharpQueue",
            _ => "javaQueue"
        };

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null);

        List<TestCaseDto> testCaseDtos = testCases.Select(tc => new TestCaseDto
        {
            Arguments = tc.Arguments,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();

        var correlationId = Guid.NewGuid().ToString();
        var message = new CodeExecutionRequest
        {
            SourceCode = code,
            Language = language,
            IsEvaluation = isEvaluation,
            TestCases = testCaseDtos
        };

        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var properties = new BasicProperties
        {
            Persistent = true,
            CorrelationId = correlationId,
            ReplyTo = "resultsQueue"
        };

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, mandatory: true, basicProperties: properties, body: body);
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
                ResultReceived?.Invoke(result.Output, result.AllTestsPassed);
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
    public string SourceCode { get; set; }
    public string Language { get; set; }
    public bool IsEvaluation { get; set; }
    public List<TestCaseDto> TestCases { get; set; }
}

public class CodeExecutionResult
{
    public string Output { get; set; }
    public bool AllTestsPassed { get; set; }
}

public class TestCaseDto
{
    public string Arguments { get; set; }
    public string ExpectedOutput { get; set; }
}