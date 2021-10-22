using System;
using System.Collections.Generic;
using AspNetCore_Client_CallAPI.Models.InputModels;
using AspNetCore_Client_CallAPI.Models.ViewModels;
using Newtonsoft.Json;
using RestSharp;

namespace AspNetCore_Client_CallAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            var client = new RestClient("https://localhost:5001/api/Authenticate/login");
            var request = new RestRequest(Method.POST);

            client.Timeout = -1;

            LoginInputModel emp = new();
            emp.username = "admin";
            emp.password = "Password123";

            string body = JsonConvert.SerializeObject(emp);

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            
            //Se si presenta l'errore: The SSL connection could not be established, dalla console lanciare il comando: dotnet dev-certs https --trust
            IRestResponse response = client.Execute(request);

            var result = JsonConvert.DeserializeObject<LoginViewModel>(response.Content);
            var token = result.token;

            var client2 = new RestClient("https://localhost:5001/api/WeatherForecast");
            client2.Timeout = -1;

            var request2 = new RestRequest(Method.GET);
            string token2 = "Bearer " + token + "";
            request2.AddHeader("Authorization", token2);

            IRestResponse response2 = client2.Execute(request2);

            List<WeatherViewModel> result2 = JsonConvert.DeserializeObject<List<WeatherViewModel>>(response2.Content);

            foreach (var item in result2)
            {
                Console.WriteLine(item.Date + " - " + item.TemperatureC + " - " + item.TemperatureF + " - " + item.Summary);
            }
        }
    }
}
