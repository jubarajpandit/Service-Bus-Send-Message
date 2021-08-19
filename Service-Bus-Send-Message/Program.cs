using System;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace Service_Bus_Send_Message
{
    class Program
    {
        // connection string to your Service Bus namespace
        const string ServiceBusConnectionString = "Endpoint=sb://demo-az.servicebus.windows.net/;SharedAccessKeyName=Send-Key;SharedAccessKey=y+WfBdLeBWWijs2VwvQlvFaBvS2Ldmca53wkZ5WzK54=";

        // name of your Service Bus topic
        const string TopicName = "demo-az-topic";

        // the client that owns the connection and can be used to create senders and receivers
        static ServiceBusClient client;

        // the sender used to publish messages to the topic
        static ServiceBusSender sender;

        // number of messages to be sent to the topic
        private const int numOfMessages = 3;
        static async Task Main()
        {
            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read
            // regularly.
            //
            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(ServiceBusConnectionString);
            sender = client.CreateSender(TopicName);

            

            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                // try adding a message to the batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus topic
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }


        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> lstUsers = new List<User>();
            for (int i = 1; i < 10; i++)
            {
                user = new User();
                user.Id = i;
                user.Name = "JPandit" + i;

                lstUsers.Add(user);
            }

            return lstUsers;
        }
    }

    public class User
    {        
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ServiceBusMessage1
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
