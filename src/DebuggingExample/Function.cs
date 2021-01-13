using System;
using System.Net;
using System.Collections.Generic;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.XRay.Recorder.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DebuggingExample
{
    public class Functions
    {
        ITimeProcessor timeProcessor = new TimeProcessor();

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public APIGatewayProxyResponse Get(
            APIGatewayProxyRequest request, ILambdaContext context)
        {
            LogMessage(context, "Processing request started.");

            APIGatewayProxyResponse response;
            try
            {
                var result = TraceFunction(timeProcessor.CurrentTimeUTC, "GetTime");
                response = CreateResponse(result);

                LogMessage(context, "Processing request succeeded.");
            }
            catch (Exception ex_1)
            {
                LogMessage(context, string.Format("Processing request failed - {0}", ex_1.Message));
                response = CreateResponse(null);
            }

            return response;
        }

        public APIGatewayProxyResponse CreateResponse(DateTime? result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            string body = (result != null) ?
                JsonSerializer.Serialize(result) :
                string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };

            return response;
        }

        private void LogMessage(ILambdaContext ctx, string msg)
        {
            ctx.Logger.LogLine(
                string.Format("{0}:{1} - {2}",
                    ctx.AwsRequestId,
                    ctx.FunctionName,
                    msg));
        }

        private T TraceFunction<T>(Func<T> func, string subSeqmentName)
        {
            AWSXRayRecorder.Instance.BeginSubsegment(subSeqmentName);
            T result = func();
            AWSXRayRecorder.Instance.EndSubsegment();

            return result;
        }
    }
}
