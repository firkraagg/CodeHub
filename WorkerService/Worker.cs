using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.RegularExpressions;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DockerClient _dockerClient;
        private string _languageQueue;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _dockerClient = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();
            _languageQueue = configuration["Language"] switch
            {
                "java" => "javaQueue",
                "csharp" => "csharpQueue",
                _ => "javaQueue"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
            factory.ClientProvidedName = "RabbitMqWorker";

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _languageQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueDeclareAsync(queue: "resultsQueue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            Console.WriteLine($"Consuming from queue: {_languageQueue}");
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                try
                {
                    var request = JsonConvert.DeserializeObject<CodeExecutionRequest>(jsonMessage);
                    if (request == null) return;

                    string output;
                    string message = "";
                    bool allPassed = false;
                    CodeExecutionResult result = new();
                    List<bool> evaluationResults = new List<bool>();
                    if (request.IsEvaluation && request.TestCases?.Any() == true)
                    {
                        int testCaseNumber = 1;
                        int secondsToTimeLimit = _languageQueue == "javaQueue" ? request.TestCases.Count * 10 : request.TestCases.Count * 20;
                        foreach (var testCase in request.TestCases)
                        {
                            output = await RunCodeInDockerContainerAsync(request.SourceCode, request.Language, secondsToTimeLimit, testCase.Arguments);

                            bool isPassed = output.Trim() == testCase.ExpectedOutput.Trim();
                            message += $"Testovací prípad èíslo {testCaseNumber} " +
                                (isPassed
                                    ? "prešiel."
                                    : $"neprešiel:" + Environment.NewLine +
                                        $"\t- Vstup: {testCase.Arguments} {Environment.NewLine}\t- Oèakávaný výstup: {testCase.ExpectedOutput} {Environment.NewLine}\t- Tvoj výstup: {output}") + Environment.NewLine;

                            evaluationResults.Add(isPassed);
                            testCaseNumber++;
                        }

                        allPassed = evaluationResults.All(x => x);
                        message += allPassed 
                            ? $"Úloha bola vypracovaná správne. Prešlo {evaluationResults.Count(x => x)} / {evaluationResults.Count} testovacích prípadov."
                            : $"Úloha nebola vypracovaná správne. Prešlo {evaluationResults.Count(x => x)} / {evaluationResults.Count} testovacích prípadov.";
                    }
                    else
                    {
                        string testArguments = request.TestCases?.FirstOrDefault()?.Arguments ?? "";
                        output = await RunCodeInDockerContainerAsync(request.SourceCode, request.Language, _languageQueue == "javaQueue" ? 15 : 30, testArguments);
                        message = $"Kód bol spustený na prvom testovacom prípade s argumentmi: {testArguments}" + Environment.NewLine + $"Výstup: {output}";
                        allPassed = true;
                    };

                    result = new CodeExecutionResult { Output = message, NumberOfPassedTests = evaluationResults.Count(x => x),  AllTestsPassed = allPassed };

                    var resultJson = JsonConvert.SerializeObject(result);
                    var resultBody = Encoding.UTF8.GetBytes(resultJson);

                    var resultProperties = new BasicProperties
                    {
                        CorrelationId = ea.BasicProperties.CorrelationId
                    };

                    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "resultsQueue", mandatory: true, basicProperties: resultProperties, body: resultBody);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(_languageQueue, autoAck: false, consumer: consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task<string> RunCodeInDockerContainerAsync(string sourceCode, string language, int timeLimitSeconds, string testCaseArguments = "")
        {
            string image = language switch
            {
                "Java" => "openjdk:21",
                "C#" => "mcr.microsoft.com/dotnet/sdk:9.0",
                _ => "openjdk:21"
            };
            string containerName = $"code-executor-{Guid.NewGuid()}";

            try
            {
                await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, new Progress<JSONMessage>());

                string methodPattern = language switch
                {
                    "Java" => @"public\s+static\s+(\w+)\s+(\w+)\s*\(([^)]*)\)",
                    "C#" => @"public\s+static\s+(\w+)\s+(\w+)\s*\(([^)]*)\)",
                    _ => ""
                };

                var methodMatch = Regex.Match(sourceCode, methodPattern);

                if (!methodMatch.Success)
                {
                    return "Neplatná hlavièka metódy. Uistite sa, že metóda je verejná, statická a správne naformátovaná.";
                }

                string returnType = methodMatch.Groups[1].Value;
                string methodName = methodMatch.Groups[2].Value;
                string parameters = methodMatch.Groups[3].Value;

                var paramList = parameters.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string parsedArguments = "";
                string argumentParsing = "";

                for (int i = 0; i < paramList.Length; i++)
                {
                    var parts = paramList[i].Trim().Split(' ');
                    string type = parts[0];
                    string paramName = parts[1];

                    argumentParsing += language switch
                    {
                        "Java" => type switch
                        {
                            "int" => $"int {paramName} = Integer.parseInt(args[{i}].trim());\n",
                            "double" => $"double {paramName} = Double.parseDouble(args[{i}].replace(\",\", \"\").trim());\n",
                            "String" => $"String {paramName} = args[{i}].trim();\n",
                            "boolean" => $"boolean {paramName} = Boolean.parseBoolean(args[{i}].trim());\n",
                            "char" => $"char {paramName} = args[{i}].trim().charAt(0);\n",
                            "long" => $"long {paramName} = Long.parseLong(args[{i}].trim());\n",
                            "int[]" => $"int[] {paramName} = Arrays.stream(args[0].split(\"\\\\s+\")).mapToInt(Integer::parseInt).toArray();\n",
                            _ => $"String {paramName} = args[{i}].trim();\n"
                        },
                        "C#" => type switch
                        {
                            "int" => $"int {paramName} = int.Parse(args[{i}].Trim());\n",
                            "double" => $"double {paramName} = double.Parse(args[{i}].Trim());\n",
                            "string" => $"string {paramName} = args[{i}].Trim();\n",
                            "boolean" => $"bool {paramName} = bool.Parse(args[{i}].Trim());\n",
                            "char" => $"char {paramName} = args[{i}].Trim()[0];\n",
                            "long" => $"long {paramName} = long.Parse(args[{i}].Trim());\n",
                            "int[]" => $"int[] {paramName} = args[{i}].Split(',').Select(int.Parse).ToArray();\n", 
                            _ => $"string {paramName} = args[{i}].Trim();\n"
                        },
                        _ => ""
                    };

                    parsedArguments += $"{paramName}, ";
                }

                parsedArguments = parsedArguments.TrimEnd(',', ' ');

                string mainMethod = language switch
                {
                    "Java" => $@"
                        public static void main(String[] args) {{
                            try {{
                                {argumentParsing}
                                System.out.println({methodName}({parsedArguments}));
                            }} catch (Exception e) {{
                                System.out.println(""Neplatný vstup alebo nedostatoèné argumenty."");
                            }}
                        }}",
                    "C#" => $@"
                        public static void Main(string[] args) {{
                            try {{
                                {argumentParsing}
                                System.Console.WriteLine({methodName}({parsedArguments}));
                            }} catch (Exception e) {{
                                System.Console.WriteLine($""Neplatný vstup alebo nedostatoèné argumenty. Chyba: {{e.Message}}"");
                            }}
                        }}",
                    _ => ""
                };

                int insertIndex = sourceCode.LastIndexOf('}');
                if (insertIndex != -1)
                    sourceCode = sourceCode.Insert(insertIndex, mainMethod);

                testCaseArguments = string.Join(" ", testCaseArguments.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));

                string runCommand = language switch
                {
                    "Java" => $"echo '{sourceCode.Replace("'", "'\\''")}' > /tmp/Code.java && javac /tmp/Code.java && java -cp /tmp Code {testCaseArguments}",
                    "C#" => $"mkdir -p /tmp/CodeProject && " +
                            $"cd /tmp/CodeProject && " +
                            $"if [ ! -f /tmp/CodeProject/CodeProject.csproj ]; then dotnet new console -n CodeProject --output . > /dev/null; fi && " +
                            $"echo '{sourceCode.Replace("'", "'\\''")}' > /tmp/CodeProject/Program.cs && " +
                            $"dotnet run --no-restore -- {testCaseArguments}",
                    _ => ""
                };

                var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = image,
                    Name = containerName,
                    HostConfig = new HostConfig
                    {
                        NetworkMode = "none",
                        Memory = 100 * 1024 * 1024,
                        CPUCount = 2,
                    },
                    Cmd = new List<string> { "sh", "-c", runCommand }
                });

                await _dockerClient.Containers.StartContainerAsync(container.ID, null);

                using var cts = new CancellationTokenSource(timeLimitSeconds * 1000);
                await Task.Delay(1000);

                try
                {
                    var logs = await _dockerClient.Containers.GetContainerLogsAsync(container.ID, false, new ContainerLogsParameters
                    {
                        ShowStdout = true,
                        ShowStderr = true,
                        Follow = true,
                        Tail = "all"
                    }, cts.Token);

                    (string stdout, string stderr) = await logs.ReadOutputToEndAsync(cts.Token);

                    await _dockerClient.Containers.RemoveContainerAsync(containerName, new ContainerRemoveParameters { Force = true });
                    return string.IsNullOrWhiteSpace(stdout) ? stderr : stdout;
                }
                catch (OperationCanceledException)
                {
                    await _dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters { WaitBeforeKillSeconds = 1 });
                    await _dockerClient.Containers.RemoveContainerAsync(containerName, new ContainerRemoveParameters { Force = true });
                    return $"Èasový limit vykonania vypršal po {timeLimitSeconds} sekundách.";
                }
            }
            catch (Exception ex)
            {
                return $"Chyba pri vykonávaní: {ex.Message}";
            }
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
        public int NumberOfPassedTests { get; set; }
        public bool AllTestsPassed { get; set; }
    }

    public class TestCaseDto
    {
        public string Arguments { get; set; }
        public string ExpectedOutput { get; set; }
    }
}