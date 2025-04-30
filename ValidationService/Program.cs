using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedModels;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Configurar exchanges
channel.ExchangeDeclare(exchange: "validation_exchange", type: ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "processed_exchange", type: ExchangeType.Direct);

// Criar filas
channel.QueueDeclare(queue: "fruit_validation", durable: true);
channel.QueueDeclare(queue: "user_validation", durable: true);

// Bindings
channel.QueueBind(queue: "fruit_validation",
                exchange: "validation_exchange",
                routingKey: "fruit_validation");

channel.QueueBind(queue: "user_validation",
                exchange: "validation_exchange",
                routingKey: "user_validation");

Console.WriteLine(" [*] Aguardando mensagens para validação...");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    try
    {
        switch (ea.RoutingKey)
        {
            case "fruit_validation":
                var fruit = JsonConvert.DeserializeObject<FruitMessage>(message);
                if (!string.IsNullOrEmpty(fruit?.FruitName))
                {
                    Console.WriteLine($" [✓] Fruta validada: {fruit.FruitName}");
                    channel.BasicPublish("processed_exchange", "valid_fruit", null, body);
                }
                break;

            case "user_validation":
                var user = JsonConvert.DeserializeObject<UserMessage>(message);
                if (user?.CPF?.Length == 11)
                {
                    Console.WriteLine($" [✓] Usuário validado: {user.FullName}");
                    channel.BasicPublish("processed_exchange", "valid_user", null, body);
                }
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [x] Erro na validação: {ex.Message}");
    }
};

channel.BasicConsume(queue: "fruit_validation", autoAck: true, consumer: consumer);
channel.BasicConsume(queue: "user_validation", autoAck: true, consumer: consumer);

Console.ReadLine();