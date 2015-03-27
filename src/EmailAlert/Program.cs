using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingService;

namespace EmailAlert
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            Console.WriteLine("I Email Alerts");
            service.HandleFanoutAlertByEmailing();
        }
    }
}
