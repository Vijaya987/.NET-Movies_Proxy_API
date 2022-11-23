using Microsoft.AspNetCore.Mvc;
using Movies_Proxy_API.Filters;
using Movies_Proxy_API.Models;
using Movies_Proxy_API.Repository.Interfaces;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace Movies_Proxy_API.Controller
{
    [Route(Constants.Route_Controller)]
    [ApiController]
    public class MongoDBController : ControllerBase
    {
        public readonly IDataRepository _dataRepository;
        public MongoDBController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [Consumes(Constants.JSON_CONTENT)]
        [Produces(Constants.JSON_CONTENT)]
        [HttpPost(Constants.GET_MOVIES)]

        [ValidateModel]
        public ActionResult Result(MoviesDataRequest movieRequest)
        {
            return _dataRepository.GetMoviesData(movieRequest);
        }


        #region IsAlive
        [ApiVersion("1.0")]
        [HttpGet("health", Name = "IsAlive")]
        public async Task<IActionResult> IsAliveAsync()
        {
            var health = await _dataRepository.IsAliveAsync();
            using (MiniProfiler.Current.Step(Constants.health))
            {
                Log.Information(Constants.health);
                return Content(JsonConvert.SerializeObject(health));
            }
        }
        #endregion
    }
}
