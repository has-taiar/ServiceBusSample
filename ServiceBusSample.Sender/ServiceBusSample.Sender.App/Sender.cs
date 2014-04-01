using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ServiceBusSample.Sender.Domain;

namespace ServiceBusSample.Sender.App
{
    public class Sender
    {
        static string _serverFqdn;
        private const int HttpPort = 9355;
        private const int TcpPort = 9354;
        private const string ServiceNamespace = "ServiceBusDefaultNamespace";
        private const string QueueName = "TALServiceBusQueue";

        public const string ExitCode = "Exit";

        static void Main(string[] args)
        {
            _serverFqdn = System.Net.Dns.GetHostEntry(string.Empty).HostName;

            var connectionStringBuilder = new ServiceBusConnectionStringBuilder {ManagementPort = HttpPort, RuntimePort = TcpPort};

            connectionStringBuilder.Endpoints.Add(new UriBuilder() { Scheme = "sb", Host = _serverFqdn, Path = ServiceNamespace }.Uri);
            connectionStringBuilder.StsEndpoints.Add(new UriBuilder() { Scheme = "https", Host = _serverFqdn, Port = HttpPort, Path = ServiceNamespace }.Uri);

            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionStringBuilder.ToString());
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionStringBuilder.ToString());

            if (namespaceManager == null)
            {
                Console.WriteLine("\nUnexpected Error");
                return;
            }

            if (namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.DeleteQueue(QueueName);
            }
            namespaceManager.CreateQueue(QueueName);

            QueueClient queueClient = messageFactory.CreateQueueClient(QueueName);
            DisplayHeader();

            var userInput = string.Empty;
            while (!ShouldExit(userInput)) 
            {
                try
                {
                    userInput = Console.ReadLine();
                    var numberOfMessages = userInput;
                    
                    var messagesList = GetListOfMessages(numberOfMessages);

                    var startTime = DateTime.Now;
                    SendMessages(messagesList, queueClient);

                    var sendingTime = (DateTime.Now - startTime).TotalSeconds;
                    Console.WriteLine(string.Format("\n{0} Messages Sent.. [Total Time: {1} Secs]", messagesList.Count(), sendingTime));

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception {0}", e.ToString());
                    throw;
                } 
            }

            if (messageFactory != null)
            {
                messageFactory.Close();
            }

        }

        private static void SendMessages(IEnumerable<ServiceBusEventMessage> messages, QueueClient queueClient)
        {
            foreach (var payLoad in messages)
            {
                var sendMessage = new BrokeredMessage(payLoad);
                queueClient.Send(sendMessage);  
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("G") + "Sending Message " + payLoad);
            }
        }

        private static IEnumerable<ServiceBusEventMessage> GetListOfMessages(string numberOfMessages)
        {
            var messages = new List<ServiceBusEventMessage>();
            var count = 0;
            int.TryParse(numberOfMessages, out count);
            for (int i = 0; i < count; i++)
            {
                messages.Add(new ServiceBusEventMessage(i));
            }
            return messages;
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("\n#######################################");
            Console.WriteLine("\nType EXIT to clean up and exit.");
            Console.WriteLine("\n#######################################");

            Console.WriteLine("\nStarting up the Queue.. ");
            Console.WriteLine("\nType Any No of mesages to send..");
        }

        private static bool ShouldExit(string userInput)
        {
            return userInput.Equals(ExitCode, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}