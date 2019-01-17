using NotLiteCode.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NaDCS.Worker
{
    public struct Payload
    {
        public object Instance;
        public Dictionary<string, MethodInfo> Methods;
    }

    public class SharedClass : IDisposable
    {
        public static Dictionary<string, Payload> Payloads = new Dictionary<string, Payload>();

        [NLCCall("ExecuteTask")]
        public static object ExecuteTask(string PayloadID, string Identifier, object[] Parameters)
        {
            if (!Payloads.TryGetValue(PayloadID, out var TargetPayload))
                throw new Exception($"Payload {PayloadID} does not exist");

            if (!TargetPayload.Methods.TryGetValue(Identifier, out var TargetMethod))
                throw new Exception($"Payload does not container method {Identifier}");

            return TargetMethod.Invoke(TargetPayload.Instance, Parameters);
        }

        [NLCCall("ExecuteTaskVoid")]
        public static object ExecuteTaskVoid(string PayloadID, string Identifier)
        {
            if (!Payloads.TryGetValue(PayloadID, out var TargetPayload))
                throw new Exception($"Payload {PayloadID} does not exist");

            if (!TargetPayload.Methods.TryGetValue(Identifier, out var TargetMethod))
                throw new Exception($"Payload does not container method {Identifier}");

            return TargetMethod.Invoke(TargetPayload.Instance, null);
        }

        [NLCCall("LoadPayload")]
        public static bool LoadPayload(string PayloadID, byte[] Payload)
        {
            if (Payloads.ContainsKey(PayloadID))
                return false;

            var TargetAssembly = Assembly.Load(Payload);

            var SharedClass = TargetAssembly.GetTypes().First(x => x.GetMethods().Any(y => y.GetCustomAttributes(typeof(NLCCall), false).Length > 0));
            var SharedInstance = Activator.CreateInstance(SharedClass);
            var SharedMethods = new Dictionary<string, MethodInfo>();

            foreach (MethodInfo SharedMethod in SharedClass.GetMethods())
            {
                var SharedMethodAttribute = SharedMethod.GetCustomAttributes(typeof(NLCCall), false);

                if (SharedMethodAttribute.Length > 0)
                {
                    var thisAttr = SharedMethodAttribute[0] as NLCCall;

                    if (SharedMethods.ContainsKey(thisAttr.Identifier))
                        continue;

                    SharedMethods.Add(thisAttr.Identifier, SharedMethod);
                }
            }

            Payloads.Add(PayloadID, new Payload() { Instance = SharedInstance, Methods = SharedMethods });

            return true;
        }

        public void Dispose()
        {
        }
    }
}