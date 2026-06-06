namespace XakUjin2026.DB
{
    public class InvalidTokenEntity
    {
        public int Id { get; set; }
        public string? TokenId { get; set; } // уникальный идентификатор токена (jti claim)
        public DateTime ExpiryDate { get; set; }
    }
}
