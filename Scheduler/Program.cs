using NotLiteCode.Network;
using NotLiteCode.Server;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NaDCS.Scheduler
{
  internal class Program
  {
    private static TaskQueue EventQueue = new TaskQueue();
    public static Server<SharedClass> Server;

    private static void Main(string[] args)
    {
      Console.Title = "NaDCS Scheduler";

      Server = new Server<SharedClass>();
      
      Server.OnServerClientConnected += (x, y) => Log($"Client {y.Client} connected!", ConsoleColor.Green);
      Server.OnServerClientDisconnected += (x, y) => Log($"Client {y.Client} disconnected!", ConsoleColor.Yellow);
      // Waiting for a Console.Write on every remote invoke can be quite taxing, so we use a simple Event Queue to make sure it doesn't lock the socket from doing other work
      Server.OnServerMethodInvoked += (x, y) => EventQueue.Enqueue(() => Task.Run(() => Log($"Client {y.Client} {(y.WasErroneous ? "failed to invoke" : "invoked")} {y.Identifier}", y.WasErroneous ? ConsoleColor.Yellow : ConsoleColor.Cyan)));
      Server.OnServerExceptionOccurred += (x, y) => Log($"Exception Occured! {y.Exception}", ConsoleColor.Red);

      Server.Start();

      Log("Scheduler Started!", ConsoleColor.Green);

      Process.GetCurrentProcess().WaitForExit();
    }

    private static readonly object WriteLock = new object();

    private static void Log(string message, ConsoleColor color)
    {
      lock (WriteLock)
      {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[{0}] ", DateTime.Now.ToLongTimeString());
        Console.ForegroundColor = color;
        Console.Write("{0}{1}", message, Environment.NewLine);
        Console.ResetColor();
      }
    }
  }

  public class TaskQueue
  {
    private Task previous = Task.FromResult(false);
    private object key = new object();

    public Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
    {
      lock (key)
      {
        var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
        previous = next;
        return next;
      }
    }

    public Task Enqueue(Func<Task> taskGenerator)
    {
      lock (key)
      {
        var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
        previous = next;
        return next;
      }
    }
  }
}