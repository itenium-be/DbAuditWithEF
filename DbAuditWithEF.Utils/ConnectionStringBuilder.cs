namespace DbAuditWithEF.Utils
{
    public static class ConnectionStringBuilder
    {
        public static string Get(string dbName)
        {
            return $"Server=localhost,5174;Database={dbName};User Id=sa;Password=password123!;TrustServerCertificate=True;";
        }
    }
}
