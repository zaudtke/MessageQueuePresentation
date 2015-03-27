using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace MessagingService
{
    public class MessageService
    {
        IModel _model;



        private readonly string HostName = "localhost";
        private readonly string UserName = "guest";
        private readonly string Password = "guest";
        private readonly bool Durable = true;

        private readonly string DirectQueue = "DirectQueue";
        private readonly string RoutingExchange = "RoutingExchange";
        private readonly string RoutingCarsQueue = "CarQueue";
        private readonly string RoutingTrucksQueue = "TruckQueue";
        private readonly string FanoutExchange = "FanoutExchange";
        private readonly string FanoutLogAlertQueue = "LoggingQueue";
        private readonly string FanoutEmailAlertQueue = "EmailQueue";
        private readonly string TopicExchange = "TopicExchange";
        private readonly string SalesQueue = "SalesQueue";
        private readonly string DiscountQueue = "DiscountQueue";
        private readonly string HeaderExchange = "HeaderExchange";
        private readonly string AllQueue = "AllQueue";
        private readonly string AnyQueue = "Queue";


        public MessageService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = HostName,
                UserName = UserName,
                Password = Password
            };

            _model = factory.CreateConnection().CreateModel();
        }


        #region ** Direct Exchange No Routing **

        public void SetupDirectDemo()
        {
            //Use Default Exchange therefore the queuename through convention is the routing key
            _model.QueueDeclare(DirectQueue, Durable, false, false, null);
        }

        public void SendDirectMessage(string message)
        {
            var basicProperties = CreateDurableBasicProperties();
            var payload = CreatePayload(message);
            //Use Empty string for Exchange to use default exchange
            _model.BasicPublish("", DirectQueue, basicProperties, payload);
        }

        public void HandleDirectMessage()
        {
            _model.BasicQos(0, 1, false);
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(DirectQueue, false, consumer);

            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue();
                String message = GetPayload(deliveryArguments.Body);
                Console.WriteLine("Message received: {0}", message);
                _model.BasicAck(deliveryArguments.DeliveryTag, false);
            }
        }

        #endregion

        #region ** Direct Exchange with Routing **

        public void SetupRoutingDemo()
        {
            _model.ExchangeDeclare(RoutingExchange, ExchangeType.Direct, true);
            _model.QueueDeclare(RoutingCarsQueue, Durable, false, false, null);
            _model.QueueDeclare(RoutingTrucksQueue, Durable, false, false, null);
            _model.QueueBind(RoutingCarsQueue, RoutingExchange, "cars");
            _model.QueueBind(RoutingTrucksQueue, RoutingExchange, "trucks");
        }

        public void SendRoutingMessage(string routingKey, string message)
        {
            var basicProperties = CreateDurableBasicProperties();
            var payload = CreatePayload(message);
            _model.BasicPublish(RoutingExchange, routingKey, basicProperties, payload);
        }

        public void HandleCars()
        {
            _model.BasicQos(0, 1, false);
            Subscription sub = new Subscription(_model, RoutingCarsQueue, false);
            while (true)
            {
                BasicDeliverEventArgs deliveryArgs = sub.Next();
                string message = GetPayload(deliveryArgs.Body);
                Console.WriteLine("Message: {0}", message);
                sub.Ack(deliveryArgs);
            }
        }

        public void HandleTrucks()
        {
            _model.BasicQos(0, 1, false);
            Subscription sub = new Subscription(_model, RoutingTrucksQueue, false);
            while (true)
            {
                BasicDeliverEventArgs deliveryArgs = sub.Next();
                string message = GetPayload(deliveryArgs.Body);
                Console.WriteLine("Message: {0}", message);
                sub.Ack(deliveryArgs);
            }
        }


        #endregion

        #region ** Fanout Exchange **

        public void SetupFanoutDemo()
        {
            _model.ExchangeDeclare(FanoutExchange, ExchangeType.Fanout, true);
            _model.QueueDeclare(FanoutLogAlertQueue, Durable, false, false, null);
            _model.QueueDeclare(FanoutEmailAlertQueue, Durable, false, false, null);
            _model.QueueBind(FanoutLogAlertQueue, FanoutExchange, "");
            _model.QueueBind(FanoutEmailAlertQueue, FanoutExchange, "");
        }

        public void SendFanoutMessage(string message)
        {
            var basicProperties = CreateDurableBasicProperties();
            var payload = CreatePayload(message);
            _model.BasicPublish(FanoutExchange, string.Empty, basicProperties, payload);
        }

        public void HandleFanoutAlertByLogging()
        {
            _model.BasicQos(0, 1, false);
            Subscription subscription = new Subscription(_model, FanoutLogAlertQueue, false);
            while (true)
            {
                BasicDeliverEventArgs deliveryArgs = subscription.Next();
                string message = GetPayload(deliveryArgs.Body);
                Console.WriteLine("Logging Alert: {0}", message);
                subscription.Ack(deliveryArgs);
            }
        }

        public void HandleFanoutAlertByEmailing()
        {
            _model.BasicQos(0, 1, false);
            Subscription subscription = new Subscription(_model, FanoutEmailAlertQueue, false);
            while (true)
            {
                BasicDeliverEventArgs deliveryArgs = subscription.Next();
                string message = GetPayload(deliveryArgs.Body);
                Console.WriteLine("Sending Alert: {0}", message);
                subscription.Ack(deliveryArgs);
            }
        }

        #endregion

        #region ** Topic Exchange **

        public void SetUpTopicExchangeDemo()
        {
            _model.ExchangeDeclare(TopicExchange, ExchangeType.Topic, Durable);
            _model.QueueDeclare(SalesQueue, Durable, false, false, null);
            _model.QueueDeclare(DiscountQueue, Durable, false, false, null);

            // * replace one word
            // # replace 0 or more words
            _model.QueueBind(SalesQueue, TopicExchange, "#.sales.#");
            _model.QueueBind(DiscountQueue, TopicExchange, "discount.#");
            
        }

        public void SendTopicMessage(string routingKey, string message)
        {
            var basicProperties = CreateDurableBasicProperties();
            var payload = CreatePayload(message);
            _model.BasicPublish(TopicExchange, routingKey, basicProperties, payload);
        }

        public void HandleSales()
        {
            _model.BasicQos(0, 1, false);
            Subscription sub = new Subscription(_model, SalesQueue, false);
            while (true)
            {
                BasicDeliverEventArgs args = sub.Next();
                string message = GetPayload(args.Body);
                Console.WriteLine("Sales For: {0}", message);
                sub.Ack(args);
            }
        }

        public void HandleDiscounts()
        {
            _model.BasicQos(0, 1, false);
            Subscription sub = new Subscription(_model, DiscountQueue, false);
            while (true)
            {
                BasicDeliverEventArgs args = sub.Next();
                string message = GetPayload(args.Body);
                Console.WriteLine("Discount For: {0}", message);
                sub.Ack(args);
            }
        }

        #endregion


        #region ** Headers Exchange **

        public void SetupHeadersDemo()
        {
            _model.ExchangeDeclare(HeaderExchange, ExchangeType.Headers, Durable);
            _model.QueueDeclare(AllQueue, Durable, false, false, null);
            _model.QueueDeclare(AnyQueue, Durable, false, false, null);
            

            Dictionary<string, object> bindingAllHeaders = new Dictionary<string, object>();
            bindingAllHeaders.Add("x-match", "all");
            bindingAllHeaders.Add("sport", "football");
            bindingAllHeaders.Add("division", "nfcnorth");
            _model.QueueBind(AllQueue, HeaderExchange, "", bindingAllHeaders);

            Dictionary<string, object> bindingAnyHeaders = new Dictionary<string, object>();
            bindingAnyHeaders.Add("x-match", "any");
            bindingAnyHeaders.Add("sport", "football");
            bindingAnyHeaders.Add("division", "nfcnorth");
            _model.QueueBind(AnyQueue, HeaderExchange, "", bindingAnyHeaders);
        }

        public void SendHeaderMessage(string message, Dictionary<string,object> headers)
        {
            var basicProperties = CreateDurableBasicProperties();
            basicProperties.Headers = headers;
            var payload = CreatePayload(message);
            _model.BasicPublish(HeaderExchange, "", basicProperties, payload);
        }

        public void HandlAllHeaders()
        {
            _model.BasicQos(0, 1, false);
            Subscription sub = new Subscription(_model, AllQueue, false);
            while (true)
            {
                BasicDeliverEventArgs args = sub.Next();
                StringBuilder builder = new StringBuilder();
                string message = GetPayload(args.Body);
                builder.Append("Message From All Queue: ").Append(message).Append(".  ");
                foreach (string key in args.BasicProperties.Headers.Keys)
                {
                    byte[] value = args.BasicProperties.Headers[key] as byte[];
                    builder.Append("Header key: ").Append(key).Append(", value: ").Append(GetPayload(value)).Append("; ");
                }
                Console.WriteLine(builder.ToString());
                sub.Ack(args);
            }
        }

        public void HandleAnyHeaders()
        {
            Subscription sub = new Subscription(_model, AnyQueue, false);
            while (true)
            {
                BasicDeliverEventArgs args = sub.Next();
                StringBuilder builder = new StringBuilder();
                string message = GetPayload(args.Body);
                builder.Append("Message From Any Queue: ").Append(message).Append(".  ");
                foreach (string key in args.BasicProperties.Headers.Keys)
                {
                    byte[] value = args.BasicProperties.Headers[key] as byte[];
                    builder.Append("Header key: ").Append(key).Append(", value: ").Append(GetPayload(value)).Append("; ");
                }
                Console.WriteLine(builder.ToString());
                sub.Ack(args);
            }
        }

        #endregion


        private IBasicProperties CreateDurableBasicProperties()
        {
            var properties = _model.CreateBasicProperties();
            properties.SetPersistent(Durable);
            return properties;
        }

        private byte[] CreatePayload(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
        private string GetPayload(byte[] body)
        {
            return Encoding.UTF8.GetString(body);
        }
    }
}
