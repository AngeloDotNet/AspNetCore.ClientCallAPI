using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore_Client_CallAPI.Models.InputModels;
using AspNetCore_Client_CallAPI.Models.ViewModels;
using RestSharp;

namespace AspNetCore_Client_CallAPI;

class Program
{
    static async Task Main(string[] args)
    {
        // Single RestClient with base URL reused for all requests -> connection pooling, fewer TLS handshakes
        var clientOptions = new RestClientOptions("https://localhost:5001")
        {
            // RestSharp uses Timeout (TimeSpan) for global max timeout for requests
            Timeout = TimeSpan.FromMilliseconds(3000)
        };

        var client = new RestClient(clientOptions);
        // Build login payload
        var loginModel = new LoginInputModel
        {
            Username = "admin",
            Password = "Password123"
        };

        // Use endpoint-specific requests but reuse the same client
        var loginRequest = new RestRequest("api/Authenticate/login", Method.Post);
        // Let RestSharp serialize the body, this avoids double allocation of serialized string.
        loginRequest.AddJsonBody(loginModel);

        try
        {
            // Use async calls to avoid blocking thread pool threads and to scale better
            var loginResponse = await client.ExecuteAsync(loginRequest, CancellationToken.None).ConfigureAwait(false);

            if (!loginResponse.IsSuccessful)
            {
                Console.WriteLine($"Login failed: {loginResponse.StatusCode} - {loginResponse.Content}");
                return;
            }

            var loginResult = JsonSerializer.Deserialize<LoginViewModel>(loginResponse.Content, DependencyInjection.jsonOptions);
            if (loginResult is null)
            {
                Console.WriteLine("Failed to parse login response.");
                return;
            }

            var token = loginResult.Token;
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Login returned empty token.");
                return;
            }

            var weatherRequest = new RestRequest("api/WeatherForecast", Method.Get);
            weatherRequest.AddHeader("Authorization", $"Bearer {token}");

            var weatherResponse = await client.ExecuteAsync(weatherRequest, CancellationToken.None).ConfigureAwait(false);

            if (!weatherResponse.IsSuccessful)
            {
                Console.WriteLine($"Request failed: {weatherResponse.StatusCode} - {weatherResponse.Content}");
                return;
            }

            var forecasts = JsonSerializer.Deserialize<List<WeatherViewModel>>(weatherResponse.Content, DependencyInjection.jsonOptions);
            if (forecasts is null)
            {
                Console.WriteLine("Failed to parse weather response.");
                return;
            }

            foreach (var item in forecasts)
            {
                // Use interpolation (single allocation per line) and a compact date format
                Console.WriteLine($"{item.Date:yyyy-MM-dd} - {item.TemperatureC} - {item.TemperatureF} - {item.Summary}");
            }
        }
        catch (Exception ex)
        {
            // Keep errors visible during development; consider structured logging for production
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}