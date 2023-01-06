using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using DigitalTwinTests.Models;
using Newtonsoft.Json;

namespace DigitalTwinTests
{
    public static class DigitalTwinSerializer
    {
        public static T Deserialize<T>(BasicDigitalTwin twin, string contentKey = null)
        {
            if (string.IsNullOrEmpty(contentKey))
            {
                var json = JsonConvert.SerializeObject(twin.Contents);
                return JsonConvert.DeserializeObject<T>(json);
            }

            var contentJson = ((System.Text.Json.JsonElement)twin.Contents[contentKey]).GetRawText();
            return JsonConvert.DeserializeObject<T>(contentJson);
        }
    }
}
