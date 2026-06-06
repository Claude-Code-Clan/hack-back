namespace XakUjin2026.Model.ExternalRequest.Building
{
    public class Building
    {
        public int? id { get; set; }
        public string? title { get; set; }
        public string? alias { get; set; }
        public int? floor { get; set; }
        public string? start_work_time { get; set; }
        public string? end_work_time { get; set; }
        public string? fias { get; set; }
        public int? apartmentCount { get; set; }
        public int? entranceCount { get; set; }
        public Address? address { get; set; }
        public bool sell_enabled { get; set; }
        public List<string>? sell_emails { get; set; }
        public string? security_number { get; set; }
        public string? ext_guid { get; set; }
        public int? guest_scud_pass_limit { get; set; }
        public bool buildings_properties_rent_available { get; set; }
        public string? resident_request_variant { get; set; }
        public string? apartment_plan_default { get; set; }
        public Meters? meters { get; set; }
        public bool paid_tickets_allowed { get; set; }
    }
}