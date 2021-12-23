using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApiProject.Interfaces
{
    public interface IWeatherDataService
    {
        Task<string> GetLatestWheatherData(string location);
    }
}
