using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Extensions.Options;
using NewShoreAir.Models.Entities;
using Microsoft.Extensions.Configuration;
namespace NewShoreAir.Dal
{
    public class DalFlight
    {
        private readonly string _stringConnection;

        public DalFlight() {

            _stringConnection = @"Server=milepola\sqlexpress;Database=NewShoreAir;Uid=sa;Pwd=Passw0rd.";

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