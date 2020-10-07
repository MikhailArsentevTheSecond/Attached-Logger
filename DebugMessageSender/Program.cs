using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace DebugMessageSender
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var pipeClient = new NamedPipeClientStream(".", "DebugPipe", PipeDirection.Out, PipeOptions.Asynchronous))
            {
                Console.WriteLine("Waiting for client connection...");
                pipeClient.Connect();
                Console.WriteLine("Client connected");
                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    while (true)
                    {
                        Console.WriteLine("Write something");
                        var input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("I send something");
                            writer.WriteLine(input);
                            writer.Flush();
                        }
                    }
                }
            }
        }
    }
}
