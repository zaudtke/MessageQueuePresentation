using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace HandleAllHeaders
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();

            Console.WriteLine("All Headers must match.  sport = football and division = nfcnorth");
            service.HandlAllHeaders();
        }
    }
}
