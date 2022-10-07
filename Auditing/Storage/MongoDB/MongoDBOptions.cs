namespace Auditing.Storage.MongoDB;

/// <summary>
/// MongoDB配置
/// </summary>
public class MongoDBOptions
{
    /// <summary>
    /// Gets or sets the database name to use when creating database objects.
    /// Default value: "cap"
    /// </summary>
    public string DatabaseName { get; set; } = "auditing";

    /// <summary>
    /// MongoDB database connection string.
    /// Default value: "mongodb://localhost:27017"
    /// </summary>
    public string DatabaseConnection { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// MongoDB received message collection name.
    /// Default value: "received"
    /// </summary>
    public string Collection { get; set; } = "record";
}