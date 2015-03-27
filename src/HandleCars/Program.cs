using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace HandleCars
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            Console.WriteLine("I Handle Cars");
            service.HandleCars();
        }
    }
}
