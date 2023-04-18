using Application.Contracts;
using AutoMapper;
using Infrastructure.Contracts;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CacheServices : ICacheServices
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IConnectionMultiplexer _redis;
        public CacheServices(IRepositoryManager repository, IMapper mapper, IConnectionMultiplexer redis)
        {
            _repository = repository;
            _mapper = mapper;
            _redis = redis;
           
        }

        public async Task<T> GetData<T>(string key)
        {
            if (!_redis.IsConnected)
                throw new Exception("Cannot connect to Redis");

            var _db = _redis.GetDatabase();
            var value = await _db.StringGetAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            if (!_redis.IsConnected)
                throw new Exception("Cannot connect to Redis");

            var _db = _redis.GetDatabase();
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = await _db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public async Task<object> RemoveData(string key)
        {
            if (!_redis.IsConnected)
                throw new Exception("Cannot connect to Redis");

            var _db = _redis.GetDatabase();
            bool _isKeyExist = await _db.KeyExistsAsync(key);
            if (_isKeyExist == true)
            {
                return await _db.KeyDeleteAsync(key);
            }
            return false;
        }
    }
}
