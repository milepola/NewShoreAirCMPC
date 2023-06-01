using NewShoreAir.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewShoreAir.Business.NewShoreAir
{
    public interface IAvailableFlights
    {
        public Task<FlightRoute> GetAvailableFlights(string Origin, string Destination);
        public Task<FlightRoute> GetRouteFlight(string FlightsNewShoreAir, string Origin, string Destination);
    }
}
