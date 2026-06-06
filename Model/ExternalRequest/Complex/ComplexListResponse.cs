namespace XakUjin2026
{
    public class ComplexListResponse
    {
        public string? command { get; set; }
        public string? message { get; set; }
        public int error { get; set; }
        public ComplexListData? data { get; set; }
        public Connection? connection { get; set; }
        public string? token { get; set; }
        public string? fromdomain { get; set; }
        public string? worktime { get; set; }
        public string? worktime_tmp { get; set; }
        public string? request_uuid { get; set; }
        public string? server_time { get; set; }
        public bool is_proxy { get; set; }
    }
}
