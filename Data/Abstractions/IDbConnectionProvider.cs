namespace Boardgame.Data.Abstractions;

/// <summary>
/// Provides database connection configuration.
/// Implementations can read from config files, environment vars, etc.
/// </summary>
public interface IDbConnectionProvider
{
    string GetConnectionString();
}

/// <summary>
/// Default implementation connecting to 'boardmaster' Docker container.
/// Database: boardgame, Port: 1435
/// </summary>
public sealed class DefaultConnectionProvider : IDbConnectionProvider
{
    private const string DefaultConnection =
        "Server=localhost,1435;Database=boardgame;User Id=sa;Password=BoardMaster1@3;" +
        "TrustServerCertificate=True;Encrypt=False;";

    public string GetConnectionString() => DefaultConnection;
}
