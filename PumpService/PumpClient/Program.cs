using PumpClient.PumpServiceReference;
using System;
using System.IO;
using System.ServiceModel;

namespace PumpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InstanceContext instanceContext = new InstanceContext(new CallbackHandler());
            PumpServiceClient client = new PumpServiceClient(instanceContext);

            client.UpdateAndCompileScript(
                Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Scripts", "Sample.script"));

            client.RunScript();

            Console.WriteLine("Нажми Enter для выхода");
            Console.ReadKey(true);

            client.Close();
        }
    }
}
