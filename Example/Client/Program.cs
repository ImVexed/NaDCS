using NaDCS;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SampleClient
{
    internal class Program
    {
        private static async Task<string> JustATest() =>
          await client.ExecuteTask<string>("work", "JustATest");

        private static Client client;

        private static void Main(string[] args)
        {
            client = new Client("localhost", 1337);

            Console.WriteLine("Client connected");

            if (client.PropagatePayload("work", File.ReadAllBytes("WorkToDo.dll")).Result)
            {
                Console.WriteLine("Payload propagated");

                for (var i = 0; i < 10; i++)
                    Task.Run(async () => Console.WriteLine(await JustATest()));
            }
            Console.WriteLine("Done");
            Console.ReadKey(true);
        }
    }
}