using NewShoreAir.Dal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NewShoreAir.Business.Services;
using NewShoreAir.Models.Entities;
using System.Collections.Specialized;
using Microsoft.Extensions.Options;

namespace NewShoreAir.Business.NewShoreAir
{
    public class AvailableFlights: IAvailableFlights
    {
        SrvRecruiting _srvRecruiting;
        DalFlight _dalFlight;
        public AvailableFlights() {
            _srvRecruiting = new SrvRecruiting();
            _dalFlight = new DalFlight();
        }
        /// <summary>
        /// Obtiene la ruta de los vuelos
        /// </summary>
        /// <param name="Origin">Origen del vuelo</param>
        /// <param name="Destination">Destino del vuelo</param>
        /// <returns></returns>
        public async Task<FlightRoute> GetAvailableFlights(string Origin, string Destination)
        {
            MethodBase? Method = MethodBase.GetCurrentMethod();

            try
            {
                FlightRoute FlightRoute = _dalFlight.GetRouteExisting(Origin, Destination);

                if (FlightRoute.Flights.Count == 0)
                {
                    string FlightsNewShoreAir = await _srvRecruiting.GetFlightsNewShoreAir();

                    FlightRoute = await GetRouteFlight(FlightsNewShoreAir, Origin, Destination);

                    if (FlightRoute.Flights.Count > 0)
                    {
                        _dalFlight.SaveRouteFlight(FlightRoute);
                    }
                }
                return FlightRoute;
            }
            catch (Exception ex)
            {
                throw new Exception("[" + Method.Name + "].  " + ex.Message);

            }
        }

        /// <summary>
        /// Obtiene todos los vuelos asociados a la solicitud
        /// </summary>
        /// <param name="Origin">Origen del vuelo</param>
        /// <param name="Destination">Destino del vuelo</param>
        /// <returns>la ruta de los vuelos</returns>
        public async Task<FlightRoute> GetRouteFlight(string FlightsNewShoreAir, string Origin, string Destination)
        {
            MethodBase? Method = MethodBase.GetCurrentMethod();

            try
            {
                List<FlightNewShoreAir>? FlightsJson = JsonConvert.DeserializeObject<List<FlightNewShoreAir>>(FlightsNewShoreAir);
                List<Flight> LstFlight = new List<Flight>();
                decimal Price = 0;
                string? NewOrigin = Origin;

                // Buscar vuelos recursivos hasta el destino
                while (NewOrigin != Destination)
                {
                    // Buscar el siguiente vuelo con origen en el último arribo 
                    FlightNewShoreAir? RouteFlight = FlightsJson.FirstOrDefault(f => f.DepartureStation == NewOrigin);

                    if (RouteFlight == null)
                    {
                        break;
                    }

                    Price = Price + RouteFlight.Price;

                    Flight NewFlight = new Flight
                    {
                        Origin = RouteFlight.DepartureStation,
                        Destination = RouteFlight.ArrivalStation,
                        Price = RouteFlight.Price,
                        Transport = new Transport
                        {
                            FlightCarrier = RouteFlight.FlightCarrier,
                            FlightNumber = RouteFlight.FlightNumber
                        }
                    };

                    // Adiciona el vuelo
                    LstFlight.Add(NewFlight);
                    //Asigna el nuevo origen
                    NewOrigin = RouteFlight.ArrivalStation;
                }

                FlightRoute flightRoute = new FlightRoute
                {
                    Origin = Origin,
                    Destination = Destination,
                    Flights = LstFlight,
                    Price= Price
                };

                return flightRoute;
            }
            catch (Exception ex)
            {
                throw new Exception("[" + Method.Name + "].  " + ex.Message);

            }
        }

        
    }
}
