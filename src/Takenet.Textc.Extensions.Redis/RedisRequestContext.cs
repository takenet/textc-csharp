using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Textc.Extensions.Redis
{
    /// <summary>
    /// Defines a context that uses redis to store variables.
    /// </summary>
    public class RedisRequestContext : IRequestContext, IDisposable
    {
        private readonly IDatabase _db;
        private ConnectionMultiplexer _redis;

        /// <summary>
        ///  Initializes a new instance of the Takenet.Textc.Extensions.Redis.RedisRequestContext class.
        /// </summary>
        /// <param name="endpoint">Redis endpoint</param>
        /// <param name="database">Redis database</param>
        /// <param name="expireTime">Redis variable validity</param>
        /// <param name="key">Redis hash key</param>
        public RedisRequestContext(string endpoint, int database, TimeSpan expireTime, string key)
            : this(CultureInfo.InvariantCulture, endpoint, database, expireTime, key)
        {

        }

        public RedisRequestContext(CultureInfo culture, string endpoint, int database, TimeSpan expireTime, string key)
        {
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            Culture = culture;
            Database = database;
            ExpireTime = expireTime;
            Endpoint = endpoint;
            Key = key;

            _redis = ConnectionMultiplexer.Connect(endpoint);
        }

        public CultureInfo Culture { get; }

        public string Endpoint { get; }

        public int Database { get; }

        public TimeSpan ExpireTime { get; }

        public string Key { get; }

        public virtual void SetVariable(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var db = _redis.GetDatabase(Database);

            db.HashSet($"user:{Key}", new HashEntry[] { new HashEntry(name, JsonConvert.SerializeObject(value)) });
            db.KeyExpire(name, ExpireTime);
            Trace.TraceInformation($"Set Variable 'user:{Key} {name}' succeeded");
        }

        public virtual object GetVariable(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var db = _redis.GetDatabase(Database);
            Trace.TraceInformation($"Getting variable 'user:{Key} {name}'");
            var value = db.HashGet($"user:{Key}", name);
            return JsonConvert.DeserializeObject(value);
        }

        public virtual void RemoveVariable(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            var db = _redis.GetDatabase(Database);
            db.HashDelete($"user:{Key}", name, CommandFlags.FireAndForget);
        }

        public void Clear()
        {
            var db = _redis.GetDatabase(Database);
            db.KeyDelete($"user:{Key}");
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
