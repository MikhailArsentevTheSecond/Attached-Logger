using CommonValues;
using System;
using System.IO;
using System.IO.Pipes;

namespace AttachedLogger
{
    public static class Logger
    {
        enum ExceptionPlace
        {
            Initialization,
            WhileSendingMessage
        }

        const string logTimeMarker = "yyyyMMdd_HHmmss";

        static bool isCrashed = false;

        static NamedPipeClientStream sender;
        static StreamWriter sendHelper;

        public static void Initialize()
        {
            try
            {
                sender = new NamedPipeClientStream(".", Constants.PipeName, PipeDirection.Out, PipeOptions.Asynchronous);
                sendHelper = new StreamWriter(sender);
                sender.Connect();
            }
            catch (Exception ex)
            {
                HandleError(ex, ExceptionPlace.Initialization);
            }
        }

        public static void SendMessage(string message)
        {
            if(PossibilityCheck())
            {
                try
                {
                    sendHelper.WriteLine(message);
                    sendHelper.Flush();
                }
                catch (Exception ex)
                {
                    HandleError(ex, ExceptionPlace.WhileSendingMessage);
                }
            }
        }

        static bool PossibilityCheck()
        {
            if(isCrashed)
            {
                return false;
            }
            else
            {
                if(sender == null)
                {
                    Initialize();
                    return true;
                }
                return true;
            }
        }

        static void HandleError(Exception ex, ExceptionPlace location)
        {
            try
            {
                isCrashed = true;
                if (sender != null)
                {
                    sender.Dispose();
                    sender = null;
                }
                if (sendHelper != null)
                {
                    sendHelper.Dispose();
                    sendHelper = null;
                }

                File.WriteAllText($"AttachedLoggerErrorLog{DateTime.Now.ToString(logTimeMarker)}.txt",
                    $"Exception location: {location}" +
                    $"\n{ex.GetType()}: {ex.Message}" +
                    $"\n{ex.StackTrace}" +
                    $"\n{ex}");
            }
            catch (Exception)
            {
                isCrashed = true;
            }
        }
    }
}
