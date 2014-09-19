using System;
using NServiceBus;
using NLog;

namespace MySaga
{
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization, IWantToRunWhenBusStartsAndStops
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
       
        public void Init()
        {

            // Log the Bus
            SetLoggingLibrary.NLog();

            
            // License this instance
            NServiceBus.Configure.Instance
              .LicensePath(@"C:\NServiceBus\License\license.xml");

            logger.Info("--------MySaga Configure IBus-------");
  
            Configure.With()
                .DefaultBuilder()  // Autofac Default Container
                .UseTransport<Msmq>()  // MSMQ, will create Queues, Defualt
                .MsmqSubscriptionStorage() // Create a subscription endpoint
                .InMemorySagaPersister()   //In memory implementation of ISagaPersister
                .UseInMemoryTimeoutPersister() //UseInMemoryTimeoutPersister, UseRavenTimeoutPersister
                .UnicastBus(); // Create the default unicast Bus

  
            logger.Info("--------MySaga Saga Enabled--------");
        
        }

        public void Start()
        {
            Console.WriteLine("This is the process hosting the saga.");
        }

        public void Stop()
        {
            Console.WriteLine("Stopped.");
        }
    }
}
