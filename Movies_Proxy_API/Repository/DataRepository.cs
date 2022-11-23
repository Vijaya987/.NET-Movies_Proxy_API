using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Movies_Proxy_API.Helpers;
using Movies_Proxy_API.Middleware;
using Movies_Proxy_API.Models;
using Movies_Proxy_API.Repository.Interfaces;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace Movies_Proxy_API.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly IMongoCollection<movies> _data;
        private readonly IOptions<DatabaseSettings> _settings;

        public DataRepository(IOptions<DatabaseSettings> settings, IOptions<UserSettings> config)
        {
            _settings = settings;
            var connection = new MongoClient(_settings.Value.ConnectionString);
            var database = connection.GetDatabase(_settings.Value.DatabaseName);
            _data = database.GetCollection<movies>(_settings.Value.CollectionName);

        }

        public ActionResult GetMoviesData(MoviesDataRequest movieRequest)
        {
            return Get(movieRequest.movie_id);

            var payload = JsonConvert.SerializeObject(movieRequest);
            var transId = Guid.NewGuid().ToString();
            Console.WriteLine(AuditMiddleware.Logger);
            if (AuditMiddleware.Logger != null)
            {
                AuditLogger.RequestInfo(
                    transId, Constants.Get, Constants.Path, string.Empty, movieRequest.ToString());
            }
            if (AuditMiddleware.Logger != null)
            {
                AuditLogger.ResponseInfo(transId, Constants.Get, Constants.Path, string.Empty, _settings.Value.DatabaseName, 
                    _settings.Value.CollectionName, payload);
            }
            Log.Information("Request: {0}", JsonConvert.SerializeObject(movieRequest));
        }

        public ActionResult Get(int id)
        {
            if (id > 0)
            {
                Log.Information("MongoDatabase Connected Successfully");
                List<movies> records = new List<movies>();
                List<ResponseDTO> list = new List<ResponseDTO>();
                using (MiniProfiler.Current.Step("Time taken to retrieve the data from database"))
                {
                    records = _data.Find(book => book.movie_id == id).ToList();

                    if (records.Count !=0)
                    {
                        Log.Information("Data is found in the MongoDB for the given movie id");
                        
                        foreach (var record in records)
                        {
                            ResponseDTO res = new ResponseDTO();
                            res.adult = record.adult;
                            res.backdrop_path = record.backdrop_path;
                            res.budget = record.budget;
                            res.homepage = record.homepage;
                            res.original_title = record.original_title;
                            res.overview = record.overview;
                            res.popularity = record.popularity;
                            res.poster_path = record.poster_path;
                            res.Title = record.Title;
                            res.movie_id = record.movie_id;
                            res.status = record.status;
                            res.tagline = record.tagline;
                            res.vote_count = record.vote_count;
                            res.vote_average = record.vote_average;
                            res.video = record.video;
                            res.revenue = record.revenue;
                            res.runtime = record.runtime;
                            res.release_date = record.release_date.ToString();
                            list.Add(res);
                        }
                        Console.WriteLine("Movie id is "+ id);
                        Console.WriteLine("Movie data is found in MongoDB");
                        return new ContentResult
                        {
                            Content = JsonConvert.SerializeObject(list),
                            ContentType = Constants.JSON_CONTENT,
                            StatusCode = 200
                        };
                    }

                    else
                    {
                        try
                        {
                            TMDbClient client = new TMDbClient("dbe4323e92576eb0b34a81b4158d11b2");
                            Movie movie = client.GetMovieAsync(id).Result;
                            if (movie != null)
                            {
                                var mydata = new movies
                                {

                                adult = movie.Adult,
                                backdrop_path = movie.BackdropPath,
                                budget = movie.Budget,
                                homepage = movie.Homepage,
                                original_title = movie.OriginalTitle,
                                overview = movie.Overview,
                                popularity = movie.Popularity,
                                poster_path = movie.PosterPath,
                                Title = movie.Title,
                                movie_id = movie.Id,
                                status = movie.Status,
                                tagline = movie.Tagline,
                                vote_count = movie.VoteCount,
                                vote_average = (float)movie.VoteAverage,
                                video = movie.Video,
                                revenue = movie.Revenue,
                                runtime = (int)movie.Runtime,
                                release_date = movie.ReleaseDate.ToString()
                                };

                                return new ContentResult
                                {
                                    Content = JsonConvert.SerializeObject(mydata),
                                    ContentType = Constants.JSON_CONTENT,
                                    StatusCode = 200
                                };

                                Console.WriteLine("Movie id is " + id);
                                Console.WriteLine("Movie data is found in TMDB");
                                Log.Information("Data is found in TMDB for the given movie id");
                                Log.Information("Inserting the data into mongodb");
                                var Content1 = JsonConvert.SerializeObject(mydata);
                                _data.InsertOne(mydata);
                            }
                            else
                            {
                                Console.WriteLine("Movie id is " + id);
                                Console.WriteLine("Data is not found for the given movie id");
                                Log.Information("No matches found in either MongoDB or TMDB");
                                ErrorResponse errorResponse = new ErrorResponse();
                                errorResponse.ErrorMessage = "No matches found in either MongoDB or TMDB";
                                errorResponse.StatusCode = 404;
                                return new ContentResult
                                {
                                    Content = JsonConvert.SerializeObject(errorResponse),
                                    ContentType = Constants.JSON_CONTENT,
                                    StatusCode = 404
                                };
                            }
                        }
                        catch
                        {
                            throw new System.TimeoutException();
                            //throw new Exception();
                            Log.Information("Internal server error");
                            ErrorResponse errorResponse = new ErrorResponse();
                            errorResponse.ErrorMessage = "Internal server error";
                            errorResponse.StatusCode = 500;
                            return new ContentResult
                            {
                                Content = JsonConvert.SerializeObject(errorResponse),
                                ContentType = Constants.JSON_CONTENT,
                                StatusCode = 500
                            };
                        }
                    }
                }                  
            }

            else
            {
                Console.WriteLine("Movie id is " + id);
                Console.WriteLine("The movie id is invalid");
                Log.Information("Incorrect value for movie id");
                ErrorResponse errorResponse = new ErrorResponse();
                errorResponse.ErrorMessage = "Incorrect value for movie id";
                errorResponse.StatusCode = 400;
                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(errorResponse),
                    ContentType = Constants.JSON_CONTENT,
                    StatusCode = 400
                };
            }
        }
        public async Task<bool> IsAliveAsync()
        {
            try
            {
                using (MiniProfiler.Current.Step(Constants.HealthCommand))
                {
                    await Task.Delay(1);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

            }
            return false;
        }
    }
}
