using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Configurar exchange
channel.ExchangeDeclare(exchange: "validation_exchange", type: ExchangeType.Direct);

var fruits = new List<FruitMessage>
{
    new() { FruitName = "Morango", Description = "Rico em vitamina C", Timestamp = DateTime.Now },
    new() { FruitName = "Uva", Description = "Fonte de antioxidantes", Timestamp = DateTime.Now },
    new() { FruitName = "Abacaxi", Description = "Ajuda na digestão", Timestamp = DateTime.Now }
};

foreach (var fruit in fruits)
{
    var message = JsonConvert.SerializeObject(fruit);
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(
        exchange: "validation_exchange",
        routingKey: "fruit_validation",
        basicProperties: null,
        body: body);

    Console.WriteLine($" [x] Fruta enviada: {fruit.FruitName}");
    Thread.Sleep(3000);
}