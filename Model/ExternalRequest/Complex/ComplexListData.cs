// Псевдоним: тип Item лежит в одноимённом namespace, поэтому уточняем явно.
using ItemModel = XakUjin2026.Model.ExternalRequest.Item.Item;

namespace XakUjin2026.Model.ExternalRequest.Complex
{
    public class ComplexListData
    {
        public Links? links { get; set; }
        public Meta? meta { get; set; }
        public List<ItemModel>? items { get; set; }
    }
}
