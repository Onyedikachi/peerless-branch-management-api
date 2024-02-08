
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using Retail.Branch.Core.Common;

namespace retail_teams_management_api.Controllers.v1
{
   
    public class LocationController : BaseApiController
    {
        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            string file = Path.GetFullPath( Path.Combine(Environment.CurrentDirectory, 
            "wwwroot","data",
            "countryandstate.json"
        ));
            var jsonFile = System.IO.File.ReadAllText(file);
            // Deserialize the JSON file into a C# object.
            var data = JsonConvert.DeserializeObject<CountryStateModel>(jsonFile);

            return Ok(new SuccessApiResponse<List<string>>("Countries with States",data.countries));
        }

        [HttpGet("lga")]
        public IActionResult States()
        {
            string file = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,
            "wwwroot", "data",
            "stateandlga.json"
        ));
            var jsonFile = System.IO.File.ReadAllText(file);
           
            var data = JsonConvert.DeserializeObject<StateLgaModel>(jsonFile);

            return Ok(new SuccessApiResponse<List<string>>("States with LGA", data.states));
        }
    }
}
