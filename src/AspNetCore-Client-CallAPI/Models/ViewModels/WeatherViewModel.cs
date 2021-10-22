using System;

namespace AspNetCore_Client_CallAPI.Models.ViewModels
{
    public class WeatherViewModel
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}