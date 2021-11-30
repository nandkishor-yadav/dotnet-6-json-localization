using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace dotnet_6_json_localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();

        public JsonStringLocalizer(IDistributedCache cache)
        {
            _cache = cache;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sReader = new StreamReader(str);
            using var reader = new JsonTextReader(sReader);
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;
                string? key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value, false);
            }
        }
        private string? GetString(string key)
        {
            string? relativeFilePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath = Path.GetFullPath(relativeFilePath);
            if (File.Exists(fullFilePath))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var result = GetValueFromJSON(key, Path.GetFullPath(relativeFilePath));

                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cacheKey, result);

                }
                return result;
            }
            return default;
        }
        private string? GetValueFromJSON(string propertyName, string filePath)
        {
            if (propertyName == null)
            {
                return default;
            }
            if (filePath == null)
            {
                return default;
            }
            using var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sReader = new StreamReader(str);
            using var reader = new JsonTextReader(sReader);
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }
            return default;
        }
    }
}
