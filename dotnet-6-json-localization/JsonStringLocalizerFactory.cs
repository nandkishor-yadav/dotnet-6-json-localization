﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace dotnet_6_json_localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;

        public JsonStringLocalizerFactory(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IStringLocalizer Create(Type resourceSource) =>
            new JsonStringLocalizer(_cache);

        public IStringLocalizer Create(string baseName, string location) =>
           new JsonStringLocalizer(_cache);
    }
}
