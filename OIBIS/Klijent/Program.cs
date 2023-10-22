using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Klijent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ITest> channel =
                new ChannelFactory<ITest>("ServiceName");

            ITest proxy = channel.CreateChannel();

            Console.WriteLine("Klijent uspesno pokrenut");

            while(true)
            {
                string provera = Console.ReadLine();

                string A = proxy.DodajLice(provera);

                Console.WriteLine(A);

            }

            
        }
    }
}
