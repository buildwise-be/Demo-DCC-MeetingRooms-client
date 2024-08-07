[System.Serializable]
public class DatabaseConfig
{
    public string connectionString;
    public string tableName;
}

[System.Serializable]
public class Config
{
    public DatabaseConfig DatabaseSettings;
}