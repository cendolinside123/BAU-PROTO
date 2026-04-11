namespace BAU_PROTO.Persistence
{
    public static class ConstantConfig
    {
        public const string DbConfig = "DefaultConnection";
        public const string ViewRoot = "index.html";
        public const string Sec = "bahl";
        //public const string AppConfig = $"appsettings.{ConstantConfig.env ?? ""}.json";

        public static string GetAppConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return $"appsettings.{env ?? ""}.json";
        }
    }
}
