param location string
param baseName string

var skuName = 'S1'
var skuUnits = 1
var partitionCount = 2

resource iotHub 'Microsoft.Devices/IotHubs@2021-07-02' = {
  name: '${replace(baseName, '-', '')}iothub'
  location: location
  sku: {
    name: skuName
    capacity: skuUnits
  }
  properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: partitionCount
      }
    }
  }
}

var hostName = iotHub.properties.hostName
var eventHubEndpoint = iotHub.properties.eventHubEndpoints.events.endpoint

output deploymentOutputs object = {
  iotHub: {
    hostName: hostName
    eventHubEndpointUrl: eventHubEndpoint
  }
}
