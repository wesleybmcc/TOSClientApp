using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOSClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new TOSService().Start();
            Console.ReadKey();
        }
    }
}
