﻿using System;
using System.Text;
using System.Threading.Tasks;
using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TestGrpcWithDapr.Services
{
    public class DaprClientImpl : AppCallback.AppCallbackBase
    {
        private readonly ILogger<DaprClientImpl> _logger;
        private readonly GreeterService _greeterService;
        public DaprClientImpl(ILogger<DaprClientImpl> logger, GreeterService greeterService)
        {
            _logger = logger;
            _greeterService = greeterService;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            switch (request.Method)
            {
                case "SayHello":
                    var requestString = request.Data.Value?.ToStringUtf8();
                    var requestHelloRequest = System.Text.Json.JsonSerializer.Deserialize<HelloRequest>(requestString);
                    var reply = await _greeterService.SayHello(requestHelloRequest, context);
                    var formattedReply = new JsonFormatter(new JsonFormatter.Settings(false)).Format(reply);
                    var any = new Any
                    {
                        TypeUrl = HelloReply.Descriptor.FullName,
                        Value = ByteString.CopyFrom(formattedReply, Encoding.UTF8)
                    };
                    #region Console Debug
                    Console.WriteLine($"Received request: {requestHelloRequest}");
                    Console.WriteLine($"reply from Greeter: {reply}");
                    #endregion
                    var response = new InvokeResponse()
                    {
                        Data = any
                    };

                    return response;
                default:
                    throw new Exception($"Unknown method invocation {request.Method}");
            }
        }

        public override async Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
        {
            return new ListTopicSubscriptionsResponse();
        }
    }
}
