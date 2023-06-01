namespace NewShoreAir.Models.Data
{
    public class Constants
    {
        //Url de la api de vuelos de newshore
        public const string UrlRecruiting = "https://recruiting-api.newshore.es/api/flights/0";
    }
    public class ConnectionStrings
    {
        /// <summary>
        /// Cadena conexion alistamiento
        /// </summary>
        public string NewShoreAirContext { get; set; } = string.Empty;
    }
}