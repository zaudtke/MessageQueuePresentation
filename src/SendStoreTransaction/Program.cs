using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;


namespace SendStoreTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            service.SetUpTopicExchangeDemo();

            Console.WriteLine("Enter Routing Key  and sales item.  Seperate with semicolin.");

            while (true)
            {
                while (true)
                {
                    string fullEntry = Console.ReadLine();
                    string[] parts = fullEntry.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    string key = parts[0];
                    string message = parts[1];

                    if (message.ToLower() == "q") break;

                    service.SendTopicMessage(key, message);
                }
            }
        }
    }
}
