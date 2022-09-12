﻿// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuke.Common.Gitter;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Net;

namespace Nuke.Common.Tools.Slack
{
    [PublicAPI]
    public static class SlackTasks
    {
        public static void SendSlackMessage(Configure<SlackMessage> configurator, string webhook)
        {
            SendSlackMessageAsync(configurator, webhook).Wait();
        }

        public static async Task SendSlackMessageAsync(Configure<SlackMessage> configurator, string webhook)
        {
            var message = configurator(new SlackMessage());
            var payload = JsonConvert.SerializeObject(message);

            using var client = new HttpClient();

            var response = await client.CreateRequest(HttpMethod.Post, webhook)
                .WithFormUrlEncodedContent(new Dictionary<string, string> { ["payload"] = payload })
                .GetResponseAsync();

            var responseText = await response.GetBodyAsync();
            Assert.True(responseText == "ok");
        }

        public static async Task<string> SendOrUpdateSlackMessage(Configure<SlackMessage> configurator, string accessToken)
        {
            var message = configurator(new SlackMessage());
            var response = await PostMessage(
                message.Timestamp == null
                    ? "https://slack.com/api/chat.postMessage"
                    : "https://slack.com/api/chat.update",
                message,
                accessToken);
            return response.GetPropertyStringValue("ts");
        }

        private static async Task<JObject> PostMessage(string url, object message, string accessToken)
        {
            var httpHandler = new GitterTasks.AuthenticatedHttpClientHandler(accessToken);
            using var client = new HttpClient(httpHandler);

            var payload = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.True(response.StatusCode == HttpStatusCode.OK, responseContent);

            var jobject = SerializationTasks.JsonDeserialize<JObject>(responseContent);
            var error = jobject.GetPropertyValueOrNull<string>("error");
            Assert.True(error == null, error);
            return jobject;
        }
    }

    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class SlackMessageActionButton : SlackMessageAction
    {
        [JsonProperty("type")]
        public string Type => "button";
    }
}
