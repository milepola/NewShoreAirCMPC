using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Extensions.Options;
using NewShoreAir.Models.Entities;
using NewShoreAir.Models.Data;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Reflection.Metadata;
using System.Data.Common;

namespace NewShoreAir.Dal
{
    public class DalFlight
    {
        private readonly string _stringConnection;

        public DalFlight(IOptionsMonitor<ConnectionStrings> optionsMonitor) {

            //_stringConnection = @"Server=milepola\sqlexpress;Database=NewShoreAir;Uid=sa;Pwd=Passw0rd.";

            var settings = optionsMonitor.CurrentValue;
            _stringConnection = settings.NewShoreAirContext;

        }
        /// <summary>
        /// Obtiene si la ruta ya se encuentra en bd
        /// </summary>
        /// <param name="Origin">Origen del vuelo</param>
        /// <param name="Destination">Destino del vuelo</param>
        /// <returns>bool</returns>
        public FlightRoute GetRouteExisting(string Origin, string Destination)
        {
            MethodBase? Method = MethodBase.GetCurrentMethod();

            try
            {
                string Query = "select J.Price AS PriceJ, F.Origin, F.Destination, F.Price, T.Carrier, T.NumberTr " +
                "FROM Journey J " +
                "INNER JOIN JourneyFlight JF ON JF.IdJourney = J.Id " +
                "INNER JOIN Flight F ON F.Id = JF.IdFlight " +
                "INNER JOIN Transport T ON T.Id = F.IdTransport " +
                "WHERE J.Origin = '" + Origin + "' and J.Destination = '" + Destination + "' " +
                "ORDER BY F.Id ";
                Decimal PriceJ = 0;
                using (SqlConnection connection = new SqlConnection(_stringConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            FlightRoute Journey = new FlightRoute();
                            Journey.Flights = new List<Flight>();

                            while (reader.Read())
                            {
                                Flight flight = new Flight
                                {
                                    Origin = reader.GetString(1),
                                    Destination = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Transport = new Transport
                                    {
                                        FlightCarrier = reader.GetString(4),
                                        FlightNumber = reader.GetString(5)
                                    }
                                };
                                PriceJ = reader.GetDecimal(0);

                                Journey.Flights.Add(flight);
                            }

                            Journey.Origin = Origin;
                            Journey.Destination = Destination;
                            Journey.Price = PriceJ;

                            return Journey;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[" + Method.Name + "].  " + ex.Message);

            }
        }

        /// <summary>
        /// Graba las rutas encontradas
        /// </summary>
        /// <param name="FlightRoute">Rutas encontradas</param>
        /// <returns></returns>
        public void SaveRouteFlight(FlightRoute FlightRoute)
        {
            MethodBase? Method = MethodBase.GetCurrentMethod();

            try
            {
                using (SqlConnection connection = new SqlConnection(_stringConnection))
                {
                    connection.Open();

                    // Insertar la ruta de vuelo en la base de datos
                    string insertJourney = "INSERT INTO Journey (Origin, Destination, Price) VALUES (@Origin, @Destination, @Price); SELECT SCOPE_IDENTITY();";
                    SqlCommand command = new SqlCommand(insertJourney, connection);
                    command.Parameters.AddWithValue("@Origin", FlightRoute.Origin);
                    command.Parameters.AddWithValue("@Destination", FlightRoute.Destination);
                    command.Parameters.AddWithValue("@Price", FlightRoute.Price);

                    int JourneyId = Convert.ToInt32(command.ExecuteScalar());
                    int TransportId = 0;

                    // Insertar los vuelos en la base de datos
                    foreach (var flight in FlightRoute.Flights)
                    {
                        string QueryT = "SELECT Id FROM Transport WHERE Carrier='" + 
                            flight.Transport.FlightCarrier + "' AND NumberTr = '" +
                            flight.Transport.FlightNumber + "'";

                        using (SqlCommand CommandT = new SqlCommand(QueryT, connection))
                        {

                            using (SqlDataReader reader = CommandT.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    TransportId = reader.GetInt32(0);
                                }
                                else
                                {
                                    TransportId = 0;
                                }
                            }
                        }
                        if (TransportId == 0)
                        {
                            string insertTransport = "INSERT INTO Transport (Carrier, NumberTr) " +
                                               "VALUES (@Carrier, @NumberTr); SELECT SCOPE_IDENTITY();";
                            SqlCommand TransportCommand = new SqlCommand(insertTransport, connection);
                            TransportCommand.Parameters.AddWithValue("@Carrier", flight.Transport.FlightCarrier);
                            TransportCommand.Parameters.AddWithValue("@NumberTr", flight.Transport.FlightNumber);

                            TransportId = Convert.ToInt32(TransportCommand.ExecuteScalar());

                        }
                            
                        string insertFlight = "INSERT INTO Flight (IdTransport, Origin, Destination, Price) " +
                                                   "VALUES (@IdTransport, @Origin, @Destination, @Price); SELECT SCOPE_IDENTITY();";
                        SqlCommand flightCommand = new SqlCommand(insertFlight, connection);
                        flightCommand.Parameters.AddWithValue("@IdTransport", TransportId);
                        flightCommand.Parameters.AddWithValue("@Origin", flight.Origin);
                        flightCommand.Parameters.AddWithValue("@Destination", flight.Destination);
                        flightCommand.Parameters.AddWithValue("@Price", flight.Price);

                        int FlightId = Convert.ToInt32(flightCommand.ExecuteScalar());

                        string insertJourneyFlight = "INSERT INTO JourneyFlight (IdJourney, IdFlight) " +
                                                   "VALUES (@IdJourney, @IdFlight)";
                        SqlCommand JourneyFlightCommand = new SqlCommand(insertJourneyFlight, connection);
                        JourneyFlightCommand.Parameters.AddWithValue("@IdJourney", JourneyId);
                        JourneyFlightCommand.Parameters.AddWithValue("@IdFlight", FlightId);

                        JourneyFlightCommand.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("[" + Method.Name + "].  " + ex.Message);

            }
        }
    }
}