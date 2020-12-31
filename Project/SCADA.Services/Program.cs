using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Services;

namespace SCADA.Services
{
    class Program
    {
        // For each service make new folder and instanciate service here!
        static void Main(string[] args)
        {
            Console.WriteLine("Services are working..");

            AlarmingKruncingHost ak = new AlarmingKruncingHost();
            ak.Open();

            DOMHost dom = new DOMHost();
            dom.Open();

            Console.ReadLine();
        }
    }
}
