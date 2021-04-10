using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiver
{
    class MessageReceiver
    {
        static async void LaunchServer(CancellationToken token)
        {
            try
            {
                PipeSecurity pSecure = new PipeSecurity();
                pSecure.SetAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));

                using (var pipeServer = new NamedPipeServerStream(CommonValues.Constants.PipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 100, 100, pSecure))
                {
                    Console.WriteLine("Waiting for connection...");
                    pipeServer.WaitForConnection();
                    Console.WriteLine("Connection established.");
                    using (StreamReader reader = new StreamReader(pipeServer))
                    {
                        while (true)
                        {
                            if (token.IsCancellationRequested)
                            {
                                Console.WriteLine("Canceled.");
                                return;
                            }
                            var result = await reader.ReadLineAsync();
                            if(result != null)
                            {
                                Console.WriteLine($"{DateTime.Now:HH:mm:ss:fff}:{result};");
                            }
                            await Task.Delay(10);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server Exception: ", ex.Message);
            }
        }

        static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            LaunchServer(tokenSource.Token);
            Console.WriteLine("Press 'Enter' to exit...");
            Console.WriteLine("Messages:");
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    return;
                }
            }
        }
    }
}

