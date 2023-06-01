using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewShoreAir.Models.Entities
{

    public class FlightRoute
    {
        [JsonProperty("origin")]
        public string? Origin { get; set; }
        [JsonProperty("destination")]

        public string? Destination { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("flights")]

        public List<Flight>? Flights { get; set; }
    }

    public class Flight
    {
        [JsonProperty("origin")]

        public string? Origin { get; set; }
        [JsonProperty("destination")]

        public string? Destination { get; set; }
        [JsonProperty("price")]

        public decimal Price { get; set; }
        [JsonProperty("transport")]

        public Transport? Transport { get; set; }
    }

    public class Transport
    {
        [JsonProperty("flightCarrier")]
        public string? FlightCarrier { get; set; }
        [JsonProperty("flightNumber")]
        public string? FlightNumber { get; set; }
    }

    public class FlightNewShoreAir
    {
        [JsonProperty("departureStation")]
        public string? DepartureStation { get; set; }

        [JsonProperty("arrivalStation")]
        public string? ArrivalStation { get; set; }

        [JsonProperty("flightCarrier")]
        public string? FlightCarrier { get; set; }

        [JsonProperty("flightNumber")]
        public string? FlightNumber { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }

    
}
