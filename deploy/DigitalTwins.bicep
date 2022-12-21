param location string
param baseName string

resource digitalTwinsInstance 'Microsoft.DigitalTwins/digitalTwinsInstances@2020-12-01' = {
  name: '${replace(baseName, '-', '')}adt'
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

var endpoint = digitalTwinsInstance.properties.hostName

output deploymentOutputs object = {
  adt: {
    endpointUrl: endpoint
  }
}
