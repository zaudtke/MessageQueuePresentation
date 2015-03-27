using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace HandleAnyHeaders
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();
            Console.WriteLine("Any Header must match.  sport = football or division = nfcnorth");
            service.HandleAnyHeaders();
        }
    }
}
