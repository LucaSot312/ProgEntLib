namespace ProgEntLib.Properties
{
    public class JWTSettings
    {
        public string SecretKey { get; set; }
        public int ExpiryInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
