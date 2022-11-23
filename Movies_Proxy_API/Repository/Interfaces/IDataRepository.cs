using Microsoft.AspNetCore.Mvc;
using Movies_Proxy_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies_Proxy_API.Repository.Interfaces
{
    public interface IDataRepository
    {
        ActionResult GetMoviesData(MoviesDataRequest movieRequest);
        Task<bool> IsAliveAsync();
    }
}
