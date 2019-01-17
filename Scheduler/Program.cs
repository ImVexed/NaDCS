using NotLiteCode.Server;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NaDCS.Scheduler
{
    internal class Program
    {
        public static Server<SharedClass> Server;

        private static void Main(string[] args)
        {
            Console.Title = "NaDCS Scheduler";

            Server = new Server<SharedClass>();

            Server.OnServerClientConnected += (x, y) => Log($"Client {y.Client} connected!", ConsoleColor.Green);
            Server.OnServerClientDisconnected += (x, y) => Log($"Client {y.Client} disconnected!", ConsoleColor.Yellow);
            Server.OnServerMethodInvoked += (x, y) => Log($"Client {y.Client} {(y.WasErroneous ? "failed to invoke" : "invoked")} {y.Identifier}", y.WasErroneous ? ConsoleColor.Yellow : ConsoleColor.Cyan);
            Server.OnServerExceptionOccurred += (x, y) => Log($"Exception Occured! {y.Exception}", ConsoleColor.Red);

            Server.Start();

            Log("Scheduler Started!", ConsoleColor.Green);

            Process.GetCurrentProcess().WaitForExit();
        }

        private static readonly SemaphoreSlim LogSem = new SemaphoreSlim(1, 1);

        private static async void Log(string message, ConsoleColor color)
        {
            await LogSem.WaitAsync();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[{0}] ", DateTime.Now.ToLongTimeString());
            Console.ForegroundColor = color;
            Console.Write("{0}{1}", message, Environment.NewLine);
            Console.ResetColor();

            LogSem.Release();
        }
    }
}