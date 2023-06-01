using Microsoft.AspNetCore.Mvc;
using NewShoreAir.Models.Entities;
using NewShoreAir.Business.NewShoreAir;
namespace NewShoreAir.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class NewShoreAir : ControllerBase
    {
        private readonly ILogger<NewShoreAir> _logger;
        private AvailableFlights _AvailableFlights = new AvailableFlights();

        public NewShoreAir(ILogger<NewShoreAir> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        /// <summary>
        /// Obtiene las rutas de los vuelos
        /// </summary>
        public async Task<FlightRoute> Get(string Origin, string Destination)
        {
            return await _AvailableFlights.GetAvailableFlights(Origin,Destination);
        }

    }
}