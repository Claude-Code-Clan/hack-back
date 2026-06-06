// Псевдоним: тип Complex лежит в одноимённом namespace, поэтому уточняем явно.
using ComplexModel = XakUjin2026.Model.ExternalRequest.Complex.Complex;

namespace XakUjin2026.Model.ExternalRequest.Building
{
    public class Buildings
    {
        public int? id { get; set; }
        public ComplexModel? complex { get; set; }
        public Building? building { get; set; }
        public List<Entrance>? entrances2 { get; set; }
    }
}