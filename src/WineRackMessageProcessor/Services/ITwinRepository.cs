using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;

namespace WineRackMessageProcessor.Services
{
    public interface ITwinRepository
    { 
        string GetTwinId(string query);
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
    }
}
