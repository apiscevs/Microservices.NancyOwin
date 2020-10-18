using System;

namespace LoyaltyProgramEventConsumer
{
    class Program
    {
        private static EventSubscriber subscriber;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            EventSubscriber subscriber = new EventSubscriber("127.0.0.1:5050");
            subscriber.Start();
            //Run(this);
            Console.ReadLine();
        }
    }
}
