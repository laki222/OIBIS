using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Test : ITest
    {
        public string DodajLice(string tekst)
        {
            if(tekst.Equals("admin")) {
                Console.WriteLine($"\nClient connected with message: {tekst}");
                return "Uspesna prijava";
            }
            else
            {
                Console.WriteLine($"\nClient connected with message: {tekst}");
                return "Neuspesna prijava. ";
            }
        }
    }
}
