using System.Threading.Tasks;

namespace NaDCS
{
  public class Client
  {
    public T ExecuteTask<T>(string PayloadID, string Identifier, params object[] Parameters) =>
      BaseClient.RemoteCall<T>("ExecuteTask", PayloadID, Identifier, Parameters);

    public T ExecuteTask<T>(string PayloadID, string Identifier) =>
      BaseClient.RemoteCall<T>("ExecuteTaskVoid", PayloadID, Identifier);

    public bool PropagatePayload(string PayloadID, byte[] Payload) =>
      BaseClient.RemoteCall<bool>("PropagatePayload", PayloadID, Payload);

    private NotLiteCode.Client BaseClient;

    public Client(string SchedulerIP, int SchedulerPort)
    {
      BaseClient = new NotLiteCode.Client(true);
      BaseClient.Connect(SchedulerIP, SchedulerPort);
    }
  }
}