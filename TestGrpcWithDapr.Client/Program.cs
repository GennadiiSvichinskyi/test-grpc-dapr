using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr.Client;

namespace TestGrpcWithDapr.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string AppId = args[0] ?? "testGrpcDaprService";
            string MethodName = args[1] ?? "SayHello";
            string name = args[2] ?? "Gennadii";
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            var client = new DaprClientBuilder()
                .UseJsonSerializationOptions(options)
                .Build();

            var anyReply = await client.InvokeMethodAsync<HelloRequest, HelloReply>(
                AppId, 
                MethodName, 
                new HelloRequest
                {
                    Name = name
                }
            );

            Console.WriteLine($"Reply : {anyReply.Message}");
        }
    }
}
