using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace Movies_Proxy_API.Models
{
    public class movies
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string id { get; set; }
        public string status { get; set; }
        public int movie_id { get; set; }
        public string Title { get; set; }
        public string release_date { get; set; }
        public string tagline { get; set; }
        public int vote_count { get; set; }
        public float vote_average { get; set; }
        public bool video { get; set; }
        public int runtime { get; set; }
        public long revenue { get; set; }
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public long budget { get; internal set; }
        public string homepage { get; internal set; }
        public string original_title { get; internal set; }
        public string overview { get; internal set; }
        public double popularity { get; internal set; }
        public string poster_path { get; internal set; }
    }
}
