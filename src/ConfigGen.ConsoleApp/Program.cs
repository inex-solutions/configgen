using System;
using System.Diagnostics;

namespace ConfigGen.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
