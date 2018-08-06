using System.Threading.Tasks;

namespace NaDCS
{
  public class Client
  {
    public async Task<T> ExecuteTask<T>(string PayloadID, string Identifier, params object[] Parameters) =>
      await BaseClient.RemoteCall<T>("ExecuteTask", PayloadID, Identifier, Parameters);

    public async Task<T> ExecuteTask<T>(string PayloadID, string Identifier) =>
      await BaseClient.RemoteCall<T>("ExecuteTaskVoid", PayloadID, Identifier);

    public async Task<bool> PropagatePayload(string PayloadID, byte[] Payload) =>
      await BaseClient.RemoteCall<bool>("PropagatePayload", PayloadID, Payload);

    private NotLiteCode.Client BaseClient;

    public Client(string SchedulerIP, int SchedulerPort)
    {
      BaseClient = new NotLiteCode.Client(true);
      BaseClient.Connect(SchedulerIP, SchedulerPort).Wait();
    }
  }
}