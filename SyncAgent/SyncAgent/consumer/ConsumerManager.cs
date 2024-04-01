using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SyncAgent {

    class ConsumerManager {

        private IConnection connection;
        private ConsumerOption option;

        private static ConsumerManager self = new ConsumerManager();

        private ConsumerManager() { }

        public static ConsumerManager getInstance (ConsumerOption option) {
            self.setOption(option);
            return self;
        }

        private void setOption (ConsumerOption option) {
            this.option = option;
        }

        public void consume () {
            ConnectionFactory factory;

            try {
                factory = new ConnectionFactory() {
                    HostName = option.ip,
                    Port = option.port,
                    UserName = option.id,
                    Password = option.pw
                };
                connection = factory.CreateConnection();
            }
            catch (SocketException e) {
                Log.error(e.Message);
                return;
            }

            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare(
                type: "fanout",
                exchange: "update",
                durable: true,
                autoDelete: false,
                arguments: null
            );

            string queueName = getMyIp();
            channel.QueueDeclare(
                queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            channel.QueueBind(queueName, "update", "");

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => {
                byte[] body = ea.Body.ToArray();
                option.handle.handle(channel, ea.DeliveryTag, Encoding.UTF8.GetString(body).Replace("\"", ""));
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }
        
        private string getMyIp() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            Log.error("네트워크 인터페이스에 접근하지 못함");
            throw new System.Exception();
        }
    }
}
