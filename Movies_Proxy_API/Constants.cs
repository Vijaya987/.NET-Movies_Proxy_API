using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Movies_Proxy_API
{
    public static class Constants
    {


        public const string JSON_CONTENT = "application/json";
        public const string Route_Controller = "api/[controller]";
        public const string GET_MOVIES = "getMoviesData";
        public const string Post = "Post";
        public const string Get = "Get";
        public const string Path = "api/mongodb/getMoviesData";
        public const string DatabaseSettings = "DatabaseSettings";
        public const string UserSettings = "UserSettings";
        public static string HealthCommand { get; set; }
        public static string health { get; set; }
    }
}
