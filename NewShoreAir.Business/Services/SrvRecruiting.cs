using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewShoreAir.Models.Data;
using System.Reflection;
using Newtonsoft.Json;
using NewShoreAir.Models.Entities;
using NewShoreAir.Dal;

namespace NewShoreAir.Business.Services
{
    public class SrvRecruiting
    {
        public SrvRecruiting() { }

        public async Task<string> GetFlightsNewShoreAir()
        {
            MethodBase? Method = MethodBase.GetCurrentMethod();
            try
            {
                // Instancia de HttpClient
                using (HttpClient Client = new HttpClient())
                {
                    //consume Api
                    HttpResponseMessage ResponseHttp = await Client.GetAsync(Constants.UrlRecruiting);

                    // Valida estado de la solicitud
                    if (ResponseHttp.IsSuccessStatusCode)
                    {
                        // almacena respueseta del servicio
                        string ResponseJson = await ResponseHttp.Content.ReadAsStringAsync();

                        // Deserializar el JSON 
                        //dynamic? Flights = JsonConvert.DeserializeObject(ResponseJson);

                        return ResponseJson;
                    }
                    else
                    {
                        return "";
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
