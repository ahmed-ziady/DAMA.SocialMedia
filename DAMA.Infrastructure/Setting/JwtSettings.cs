namespace DAMA.Infrastructure.Setting
{
    public class JwtSettings
    {
        public string Secret = "a4f8f9e7d2c1b3a5e9f0d4c6b8a7f2e3";
        public string Issuer  = "https://localhost:7170";
        public string Audience = "DAMAWebApi";
        public int ExpireHours = 12;
    }
}
