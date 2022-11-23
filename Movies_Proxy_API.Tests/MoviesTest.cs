using Movies_Proxy_API.Controller;
using Movies_Proxy_API.Filters;
using Movies_Proxy_API.Helpers;
using Movies_Proxy_API.Models;
using Movies_Proxy_API.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;

namespace Movies_Proxy_API.Tests
{
    [TestClass]
    public class MoviesTest
    {
        static string connection = "mongodb://w3.training5.modak.com:27017";
        static string database = "WebApi_mt4037";
        static string collection = "movies";
        static DatabaseSettings databaseSettings = new DatabaseSettings()
        {
            ConnectionString = connection,
            DatabaseName = database,
            CollectionName = collection
        };
        static AuditLog auditLog = new AuditLog();
        static UserSettings userSettings = new UserSettings()
        {
            AuditLog = auditLog
        };
        static IOptions<DatabaseSettings> options = Options.Create(databaseSettings);
        static readonly IOptions<UserSettings> config = Options.Create(userSettings);
        static DataRepository dataRepository = new DataRepository(options, config);
        static MongoDBController controller = new MongoDBController(dataRepository);

        [TestMethod]
        public void InputFound()
        {

            var request = new MoviesDataRequest
            {
                movie_id=765
            };
            var response = controller.Result(request) as ContentResult;
            Assert.AreEqual(200, response.StatusCode);
        }
        [TestMethod]
        public void InputNotFound()
        {
            var request = new MoviesDataRequest
            {
                movie_id=10
            };
            var response = controller.Result(request) as ContentResult;
            Assert.AreEqual(404, response.StatusCode);
        }
        [TestMethod]
        public void InvalidInput()
        {
            var request = new MoviesDataRequest
            {
                movie_id = -1
            };
            var response = controller.Result(request) as ContentResult;
            Assert.AreEqual(400, response.StatusCode);
        }

      
        [TestMethod]
        public void MockTesting()
        {
            var ioptions = new Mock<IOptions<DatabaseSettings>>();
            var datarepo = new Mock<DataRepository>(ioptions);
            var validationFilter = new ValidateModelAttribute();
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "movies_api_proxy");
            var actionContext = new ActionContext(
            Mock.Of<HttpContext>(),
            Mock.Of<RouteData>(),
            Mock.Of<ActionDescriptor>(),
            modelState
            );
            var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            new Mock<MongoDBController>(datarepo)
            );
            validationFilter.OnActionExecuting(actionExecutingContext);

            Assert.IsInstanceOfType(actionExecutingContext.Result, typeof(BadRequestObjectResult));
        }
       


        [TestMethod]
        
        [System.Obsolete]
        public async Task StartUpHealth_Success_Test()
        {
            // Arrange
            var projectDir = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.Development.json")
            .Build();
             var server = new TestServer(new WebHostBuilder()
            .UseContentRoot(projectDir)
            .UseConfiguration(config)
            .UseStartup<Startup>()
            .UseSerilog());

            // Act
            using (var client = server.CreateClient())
            {
                var request = new MoviesDataRequest
                {
                    movie_id = 19
                };
                string strPayload = JsonConvert.SerializeObject(request);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8,
               "application/json");
                var result = await client.PostAsync("/api/mongodb/getMoviesData", c);
                // Ensure Success StatusCode is returned from response
                _ = await result.Content.ReadAsStringAsync();
                result.EnsureSuccessStatusCode();
                // Assert
                Assert.IsTrue(result.IsSuccessStatusCode);
                Assert.AreEqual(200, (int)result.StatusCode);
            }
        }

        [TestMethod]
        [System.Obsolete]
        public async Task StartUpHealth_error404()
        {
            // Arrange
            var projectDir = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.Development.json")
            .Build();
            var server = new TestServer(new WebHostBuilder()
            .UseContentRoot(projectDir)
            .UseConfiguration(config)
            .UseStartup<Startup>()
            .UseSerilog());
            // Act
            using (var client = server.CreateClient())
            {
                var request = new MoviesDataRequest
                {
                    movie_id=10
                };
                string strPayload = JsonConvert.SerializeObject(request);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8,
               "application/json");
                var result = await client.PostAsync("/api/mongodb/getMoviesData", c);
                // Ensure Success StatusCode is returned from response
                _ = await result.Content.ReadAsStringAsync();
                Assert.AreEqual(404, (int)result.StatusCode);
            }
        }

        [TestMethod]
        [System.Obsolete]
        public async Task Health_Success_Test()
        {
            // Arrange
            var projectDir = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.Development.json")
            .Build();
            var server = new TestServer(new WebHostBuilder()
            .UseContentRoot(projectDir)
            .UseConfiguration(config)
            .UseStartup<Startup>()
            .UseSerilog());

            // Act
            using var client = server.CreateClient();
            var request = new MoviesDataRequest
            {
                movie_id = 578
            };
            string strPayload = JsonConvert.SerializeObject(request);
            _ = new StringContent(strPayload, Encoding.UTF8,
           "application/json");
            var result = await client.GetAsync("/api/mongodb/health");
            result.EnsureSuccessStatusCode();
            // Assert
            Assert.IsTrue(result.IsSuccessStatusCode);
            Assert.AreEqual(200, (int)result.StatusCode);
        }
    }
}

//code coverage command
//coverlet .\bin\Debug\netcoreapp3.1\Movies_Proxy_API.Tests.dll --target "dotnet" --targetargs "test --no-build" --exclude "[*]Movies_Proxy_API*"

//report generator command
//reportgenerator -reports:".\TestResults\76f5ae84-0fd4-470a-98d7-df44a50f0770\coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html



