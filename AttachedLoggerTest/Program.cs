using AttachedLogger;
using System;

namespace AttachedLoggerTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Write a line");
            Logger.SendMessage(Console.ReadLine());
        }
    }
}