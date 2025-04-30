using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "processed_exchange", type: ExchangeType.Direct);
var queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queue: queueName,
                exchange: "processed_exchange",
                routingKey: "valid_user");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var user = JsonConvert.DeserializeObject<UserMessage>(message);

    Console.WriteLine($" [✓] Usuário recebido: {user.FullName} - CPF: {user.CPF} ({user.RegistrationDate})");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Receiver 2 pronto. Aguardando usuários...");
Console.ReadLine();