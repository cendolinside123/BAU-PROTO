namespace BAU_PROTO.Persistence
{
    public static class ConstantConfig
    {
        public const string DbConfig = "DefaultConnection";
        public const string ViewRoot = "index.html";
        public const string Sec = "bahl";
        public const string IVFront = "IVFront";
        public const string KeyFront = "keyFront";
        public const string IV = "IV";
        public const string Key = "key";

        public static string GetAppConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return $"appsettings.{env ?? ""}.json";
        }
    }
}
