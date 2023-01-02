using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using WineRackMessageProcessor.Models;

namespace WineRackMessageProcessor.Services
{
    public interface ITwinRepository
    {
        string GetOrganizationTwinId(string name);
        Slot GetSlot(string organizationId, string deviceName, int slotNumber, bool containsBottleTwin = false);

        Task<BasicDigitalTwin> CreateOrganizationTwin(string name);
        Task UpdateTwin<T>(string id, string path, T value);
        Task<BasicDigitalTwin> CreateWineRackTwin(string serialNumber, string deviceName, int slotCount);
        Task<BasicDigitalTwin> CreateScannerTwin(string serialNumber, string name);
        Task<BasicDigitalTwin> CreateSlotTwin(int slotNumber);

        Task<BasicRelationship> CreateRelationship(
            string name,
            string sourceTwinId,
            string targetTwinId,
            IDictionary<string, object> properties = null);

        Task<BasicDigitalTwin> CreateBottleTwin(string upcCode);
        Task DeleteTwin(string twinId);

        Task<BasicRelationship> GetRelationship(string twinId);
        Task DeleteRelationship(string twinId, string relationshipId);
    }
}
