using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApiProject.Interfaces;
using WeatherApiProject.Models;

namespace WeatherApiProject.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly ILogger<WeatherDataService> _logger;
        private readonly OpenWeatherMapSettings _config;
        private readonly HttpClient _client;
        public WeatherDataService(ILogger<WeatherDataService> logger,
                IOptions<OpenWeatherMapSettings> options,
                IHttpClientFactory clientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = options.Value ?? throw new ArgumentNullException(nameof(options.Value));
            _client = clientFactory.CreateClient("Default") ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<string> GetLatestWheatherData(string location)
        {
            _logger.LogInformation($"Attempting to get weather conditions for {location} from OpenWeatherMap API.");
            var response = await Policy.Handle<HttpRequestException>()
                   .OrTransientHttpStatusCode()
                   .OrResult(msg => !msg.IsSuccessStatusCode)
                   .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(retry))
                   .ExecuteAsync(async () =>
                   {
                       var request = new HttpRequestMessage(HttpMethod.Get, $"data/2.5/weather");
                       request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                       {
                           { "q", location},
                           { "appid", _config.APIKey}
                       });
                       var response = await _client.SendAsync(request);
                       return response;
                   });
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
