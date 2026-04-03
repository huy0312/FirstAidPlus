using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace FirstAidPlus.Services
{
    public class KeepAliveService : BackgroundService
    {
        private readonly ILogger<KeepAliveService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _url = "https://firstaidplus.onrender.com/health";

        public KeepAliveService(ILogger<KeepAliveService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KeepAliveService is starting.");

            // Wait 1 minute before first ping to allow app to warm up
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("KeepAliveService is pinging {Url}", _url);
                    using var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync(_url, stoppingToken);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("KeepAliveService: Ping successful.");
                    }
                    else
                    {
                        _logger.LogWarning("KeepAliveService: Ping failed with status code {StatusCode}.", response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "KeepAliveService: Error occurred while pinging.");
                }

                // Ping every 10 minutes (Render sleeps after 15 mins)
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }

            _logger.LogInformation("KeepAliveService is stopping.");
        }
    }
}
