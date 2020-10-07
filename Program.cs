using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipes
{
    class PipeServer
    {
        static async void LaunchServer(CancellationToken token)
        {
            try
            {
                await Task.Run(() =>
                {

                    using (var pipeServer = new NamedPipeServerStream("datapipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1, 1))
                    {
                        Console.WriteLine("Waiting for conntection...");
                        pipeServer.WaitForConnection();
                        Console.WriteLine("Connection established.");
                        using (StreamReader reader = new StreamReader(pipeServer))
                        {
                            while (true)
                            {
                                if(token.IsCancellationRequested)
                                {
                                    return;
                                }
                                var result = reader.ReadLine();
                                if (!string.IsNullOrWhiteSpace(result))
                                {
                                    Console.WriteLine(result);
                                }
                                Thread.Sleep(10);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Server Exception: ", ex.Message);
            }
        }

        static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            LaunchServer(tokenSource.Token);
            while (true)
            {
                var input = Console.ReadLine();
                if(input == "close")
                {
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    return;
                }
            }
        }
    }
}
