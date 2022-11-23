namespace Movies_Proxy_API.Repository
{
    internal class ResponseDTO
    {
        public string Id { get; internal set; }
        public string Title { get; set; }
        public int movie_id { get; set; }
        public string status { get; set; }
        public string release_date { get; set; }
        public string tagline { get; set; }
        public int vote_count { get; set; }
        public float vote_average { get; set; }
        public bool video { get; set; }
        public int runtime { get; set; }
        public long revenue { get; set; }
        public bool adult { get; set; }
        public string  backdrop_path { get; internal set; }
        public long budget { get; internal set; }
        public string homepage { get; internal set; }
        public string original_title { get; internal set; }
        public string overview { get; internal set; }
        public double popularity { get; internal set; }
        public string poster_path { get; internal set; }
    }
}