using StackExchange.Redis;

using System.Collections.Concurrent;

namespace UserManagement.Infrastructure.Redis;
public class RedisConnectionPool
{
    private readonly ConcurrentBag<ConnectionMultiplexer> _pool;
    private readonly int _poolSize;
    private readonly ConfigurationOptions _redisConfiguration;

    // Static lazy instance of RedisConnectionPool
    private static Lazy<RedisConnectionPool> _instance;

    // Private constructor to initialize the pool
    private RedisConnectionPool(string connectionString, int poolSize)
    {
        _pool = new ConcurrentBag<ConnectionMultiplexer>();
        _poolSize = poolSize;

        // Parse connection string to ConfigurationOptions
        _redisConfiguration = ConfigurationOptions.Parse(connectionString);
        _redisConfiguration.Ssl = true; // Enable SSL for Azure Redis
        _redisConfiguration.AbortOnConnectFail = false;

        // Initialize the connection pool
        for (int i = 0; i < _poolSize; i++)
        {
            _pool.Add(CreateConnection());
        }
    }

    // Public method to initialize the pool instance
    public static void Initialize(string connectionString, int poolSize)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("RedisConnectionPool is already initialized.");
        }

        _instance = new Lazy<RedisConnectionPool>(() => new RedisConnectionPool(connectionString, poolSize));
    }

    // Access the instance
    public static RedisConnectionPool Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("RedisConnectionPool is not initialized. Call Initialize first.");
            }

            return _instance.Value;
        }
    }

    // Get a Redis connection from the pool
    public ConnectionMultiplexer GetConnection()
    {
        if (_pool.TryTake(out var connection))
        {
            return connection;
        }

        // Create a new connection if the pool is exhausted
        return CreateConnection();
    }

    // Return a connection back to the pool
    public void ReturnConnection(ConnectionMultiplexer connection)
    {
        if (connection.IsConnected)
        {
            _pool.Add(connection);
        }
        else
        {
            connection.Dispose();
            _pool.Add(CreateConnection());
        }
    }

    // Helper method to create a new Redis connection
    private ConnectionMultiplexer CreateConnection()
    {
        return ConnectionMultiplexer.Connect(_redisConfiguration);
    }
}
