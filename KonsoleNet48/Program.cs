using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemProcessWrapper;

namespace KonsoleNet48
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Test");

            var p = new ProcessWrapper(Processes.PasswordProperties);
            Console.ReadLine();

        }
    }
}
