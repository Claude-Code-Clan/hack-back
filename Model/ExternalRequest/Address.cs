using Microsoft.AspNetCore.Mvc;

namespace XakUjin2026.Model.ExternalRequest{
    public class Address
    {
        public string? city { get; set; }
        public string? street { get; set; }
        public string? house { get; set; }
        public string? houseNumber { get; set; }
        public string? placementNumber { get; set; }
        public string? fullAddress { get; set; }

        public string GetFullAddress()
        {
            if (!string.IsNullOrEmpty(fullAddress))
                return fullAddress;

            List<string> parts = new List<string>();
            if (!string.IsNullOrEmpty(city))
                parts.Add(city);
            if (!string.IsNullOrEmpty(street))
                parts.Add(street);
            if (!string.IsNullOrEmpty(house))
                parts.Add(house);
            if (!string.IsNullOrEmpty(houseNumber))
                parts.Add(houseNumber);
            if (!string.IsNullOrEmpty(placementNumber))
                parts.Add(placementNumber);

            return string.Join(", ", parts);
        }
    }
}