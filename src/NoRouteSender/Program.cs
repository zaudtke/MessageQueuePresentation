using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace NoRouteSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            service.SetupDirectDemo();

            Console.WriteLine("Enter Message.");

            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "q") break;
                service.SendDirectMessage(message);
            }
        }

        
    }
}
