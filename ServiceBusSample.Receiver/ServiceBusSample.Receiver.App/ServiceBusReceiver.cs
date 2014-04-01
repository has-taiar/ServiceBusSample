using System;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using ServiceBusSample.Sender.Domain;

namespace ServiceBusSample.Receiver.App
{
    public class ServiceBusReceiver
    {
        static string _serverFqdn;
        private const int HttpPort = 9355;
        private const int TcpPort = 9354;
        private const string ServiceNamespace = "ServiceBusDefaultNamespace";
        private const string QueueName = "TALServiceBusQueue";
        
        static void Main(string[] args)
        {
            _serverFqdn = System.Net.Dns.GetHostEntry(string.Empty).HostName;

            var connectionStringBuilder = new ServiceBusConnectionStringBuilder { ManagementPort = HttpPort, RuntimePort = TcpPort };

            connectionStringBuilder.Endpoints.Add(new UriBuilder() { Scheme = "sb", Host = _serverFqdn, Path = ServiceNamespace }.Uri);
            connectionStringBuilder.StsEndpoints.Add(new UriBuilder() { Scheme = "https", Host = _serverFqdn, Port = HttpPort, Path = ServiceNamespace }.Uri);

            var messageFactory = MessagingFactory.CreateFromConnectionString(connectionStringBuilder.ToString());
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionStringBuilder.ToString());

            if (namespaceManager == null)
            {
                Console.WriteLine("\nUnexpected Error");
                return;
            }

            if (!namespaceManager.QueueExists(QueueName))
            {
                Console.WriteLine(@"Error: TALServiceBusQueue Queue des not Exist... Start your Queue first");
                return;
            }

            QueueClient queueClient = messageFactory.CreateQueueClient(QueueName);

            while (true)
	        {
	            try
                {
                    // Receive the message from the queue
                    BrokeredMessage receivedMessage = queueClient.Receive();

                    if (receivedMessage != null)
                    {
                        Console.WriteLine(@"Message received: {0}", receivedMessage.GetBody<ServiceBusEventMessage>());
                        receivedMessage.Complete();
                    }
                    else
                    {
                        Console.WriteLine(@"No new messages in the queue");
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception {0}", e);
                    throw;
                }
	        }


            if (messageFactory != null)
            {
                messageFactory.Close();
            }

//            Console.WriteLine("Press  ENTER to clean up and exit.");
//            Console.ReadLine();
        }
    }
}