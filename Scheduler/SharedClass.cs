using NotLiteCode;
using NotLiteCode.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NaDCS.Scheduler
{
  public class Worker
  {
    public Client Client;
    public int Tasks;

    public Worker(Client Client)
    {
      this.Client = Client;
      this.Tasks = 0;
    }
  }

  public class SharedClass : IDisposable
  {
    public static Dictionary<EndPoint, Worker> Workers = new Dictionary<EndPoint, Worker>();
    private static object TaskingLock = new object();

    [NLCCall("PropagatePayload", true)]
    public static bool PropagatePayload(EndPoint Context, string PayloadID, byte[] Payload)
    {
      // Sanity check to make sure a worker isn't trying to distribute a payload
      if (Workers.ContainsKey(Context))
        return false;

      foreach (var worker in Workers.Values)
        if (!worker.Client.RemoteCall<bool>("LoadPayload", PayloadID, Payload).Result)
          return false;

      return true;
    }

    [NLCCall("RegisterAsWorker", true)]
    public static bool RegisterAsWorker(EndPoint Context)
    {
      if (Workers.ContainsKey(Context))
        return false;

      Program.Server.DetatchFromSocket(Program.Server.Clients[Context].Socket);

      Workers.TryAdd(Context, new Worker(new Client(Program.Server.Clients[Context].Socket, true)));

      return true;
    }

    [NLCCall("ExecuteTask", true)]
    public static object ExecuteTask(EndPoint Context, string PayloadID, string Identifier, object[] Parameters)
    {
      EndPoint LeastBusyWorker;
      lock (TaskingLock)
      {
        LeastBusyWorker = Workers.OrderBy(x => x.Value.Tasks).First().Key;

        Workers[LeastBusyWorker].Tasks++;
      }
      var Result = Workers[LeastBusyWorker].Client.RemoteCall<object>("ExecuteTask", PayloadID, Identifier, Parameters).Result;

      lock (TaskingLock)
        Workers[LeastBusyWorker].Tasks--;

      return Result;
    }

    [NLCCall("ExecuteTaskVoid", true)]
    public static object ExecuteTaskVoid(EndPoint Context, string PayloadID, string Identifier)
    {
      EndPoint LeastBusyWorker;
      lock (TaskingLock)
      {
        LeastBusyWorker = Workers.OrderBy(x => x.Value.Tasks).First().Key;

        Workers[LeastBusyWorker].Tasks++;
      }
      var Result = Workers[LeastBusyWorker].Client.RemoteCall<object>("ExecuteTaskVoid", PayloadID, Identifier).Result;

      lock (TaskingLock)
        Workers[LeastBusyWorker].Tasks--;

      return Result;
    }

    public void Dispose()
    {
    }
  }
}