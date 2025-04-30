using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "validation_exchange", type: ExchangeType.Direct);

var users = new List<UserMessage>
{
    new() {
        FullName = "João Silva",
        Address = "Rua A, 123",
        RG = "12.345.678-9",
        CPF = "123.456.789-00",
        RegistrationDate = DateTime.Now
    },
    new() {
        FullName = "Maria Souza",
        Address = "Av. B, 456",
        RG = "98.765.432-1",
        CPF = "987.654.321-00",
        RegistrationDate = DateTime.Now
    }
};

foreach (var user in users)
{
    var message = JsonConvert.SerializeObject(user);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "validation_exchange",
        routingKey: "user_validation",
        basicProperties: null,
        body: body);

    Console.WriteLine($" [x] Usuário enviado: {user.FullName}");
    Thread.Sleep(4000);
}