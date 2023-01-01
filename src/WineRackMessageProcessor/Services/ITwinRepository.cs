using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.DigitalTwins.Core;

namespace WineRackMessageProcessor.Services
{
    public interface ITwinRepository
    {
        string GetTwinId(string query);
        Task UpdateTwin<T>(string id, string path, T value);

    }

    public class TwinRepository : ITwinRepository
    {
        private readonly DigitalTwinsClient _dtClient;

        public TwinRepository(DigitalTwinsClient dtClient)
        {
            _dtClient = dtClient;
        }

        public string GetTwinId(string query)
        {
            var queryResult = this._dtClient.QueryAsync<object>(query).SingleAsync().Result;
            return ((System.Text.Json.JsonElement)queryResult).GetProperty("$dtId").GetString();
        }

        public async Task UpdateTwin<T>(string id, string path, T value)
        {
            var patchDoc = new JsonPatchDocument();
            patchDoc.AppendReplace(path, value);

            await this._dtClient.UpdateDigitalTwinAsync(id, patchDoc);
        }
    }
}
