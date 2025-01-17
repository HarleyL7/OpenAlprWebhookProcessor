﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAlprWebhookProcessor.Data;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAlprWebhookProcessor.Alerts.Pushover
{
    public class PushoverClient : IAlertClient
    {
        private const string PushOverApiUrl = "https://api.pushover.net/1/messages.json";

        private const string VerifyCredentialsUrl = "https://api.pushover.net/1/users/validate.json?token={0}&user={1}";

        private readonly HttpClient _httpClient;

        private readonly IServiceProvider _serviceProvider;

        public PushoverClient(IServiceProvider serviceProvider)
        {
            _httpClient = new HttpClient();
            _serviceProvider = serviceProvider;
        }

        public async Task SendAlertAsync(
            Alert alert,
            string base64PreviewJpeg,
            CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PushoverClient>>();

                logger.LogInformation("Sending Alert via Pushover.");

                var processorContext = scope.ServiceProvider.GetRequiredService<ProcessorContext>();

                var clientSettings = await processorContext.PushoverAlertClients.FirstOrDefaultAsync(cancellationToken);

                var boundary = Guid.NewGuid().ToString();
                using (var content = new MultipartFormDataContent(boundary))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data; boundary=" + boundary);

                    content.Add(new StringContent(clientSettings.UserKey), "user");
                    content.Add(new StringContent(clientSettings.ApiToken), "token");
                    content.Add(new StringContent(alert.PlateNumber + " " + alert.Description), "message");
                    content.Add(new StringContent("openalpr alert"), "title");

                    if (clientSettings.SendPlatePreview)
                    {
                        content.Add(new ByteArrayContent(Convert.FromBase64String(base64PreviewJpeg)), "attachment", "attachment.jpg");
                    }

                    try
                    {
                        var response = await _httpClient.PostAsync(
                            PushOverApiUrl,
                            content,
                            cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadAsStringAsync(cancellationToken);

                            logger.LogError("Failed to send alert via Pushover: " + result);
                            throw new InvalidOperationException("failed");
                        }

                        logger.LogInformation("Alert sent via Pushover.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to send alert via Pushover: " + ex.Message);
                        throw new InvalidOperationException("failed");
                    }
                }
            }
        }

        public async Task VerifyCredentialsAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PushoverClient>>();

                logger.LogInformation("Testing credentials via Pushover.");

                var processorContext = scope.ServiceProvider.GetRequiredService<ProcessorContext>();

                var clientSettings = await processorContext.PushoverAlertClients.FirstOrDefaultAsync(cancellationToken);

                try
                {
                    var result = await _httpClient.PostAsync(VerifyCredentialsUrl
                            .Replace("{0}", clientSettings.ApiToken)
                            .Replace("{1}", clientSettings.UserKey),
                        null,
                        cancellationToken);

                    if (!result.IsSuccessStatusCode)
                    {
                        var message = await result.Content.ReadAsStringAsync(cancellationToken);
                        logger.LogError("Pushover credential check failed: " + message);
                    }

                    logger.LogInformation("Pushover credentials are valid.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Pushover credential check failed: " + ex.Message);
                }
            }
        }
    }
}
