using NotLiteCode.Server;
using System;
using System.Threading;

namespace WorkToDo
{
    public class Work : IDisposable
    {
        [NLCCall("JustATest")]
        public string Test()
        {
            Console.WriteLine("Hey! The client invoked me!");
            Thread.Sleep(1000);
            return "Howdy";
        }

        public void Dispose()
        {
        }
    }
}