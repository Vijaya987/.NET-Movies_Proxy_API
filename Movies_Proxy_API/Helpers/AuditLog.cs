

namespace Movies_Proxy_API.Helpers
{
    public class AuditLog
    {

        public string Path { get; set; }
        public string RollingInterval { get; set; }
        ///rolling interval
        public bool Shared { get; set; }
        public int RetainedFileCountLimit { get; set; }


    }
}
