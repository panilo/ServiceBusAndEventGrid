using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SBEventGridIntegration
{
    public class TraceReceiver
    {        
        public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            // parse query parameter
            var content = req.Content;

            string jsonContent = await content.ReadAsStringAsync();
            log.Info($"Received Event with payload: {jsonContent}");

            IEnumerable<string> headerValues;
            if (req.Headers.TryGetValues("Aeg-Event-Type", out headerValues))
            {
                var validationHeaderValue = headerValues.FirstOrDefault();
                if (validationHeaderValue == "SubscriptionValidation")
                {
                    var events = JsonConvert.DeserializeObject<GridEvent[]>(jsonContent);
                    var code = events[0].Data["validationCode"];
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { validationResponse = code }))
                    };
                }
            }

            return jsonContent == null
                ?
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Pass a name on the query string or in the request body")
                }
                :
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Hello " + jsonContent)
                };
        }

        public class GridEvent
        {
            public string Id { get; set; }
            public string EventType { get; set; }
            public string Subject { get; set; }
            public DateTime EventTime { get; set; }
            public Dictionary<string, string> Data { get; set; }
            public string Topic { get; set; }
        }
    }
}
