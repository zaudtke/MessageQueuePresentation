using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagingService;

namespace SendHeaders
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MessageService();

            service.SetupHeadersDemo();

            Console.WriteLine("Enter message format: sport.division;message");
            while (true)
            {
                string line = Console.ReadLine();
                string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string headers = parts[0];
                string[] headervalues = headers.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                Dictionary<string, object> headerDictionary = new Dictionary<string, object>();
                headerDictionary.Add("sport", headervalues[0]);
                headerDictionary.Add("division", headervalues[1]);
                string message = parts[1];
                if (message.ToLower() == "q") break;

                service.SendHeaderMessage(message, headerDictionary);
            }
        }
    }
}
