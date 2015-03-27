using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace HandleDiscounts
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            Console.WriteLine("I handle discounts");
            service.HandleDiscounts();
        }
    }
}
