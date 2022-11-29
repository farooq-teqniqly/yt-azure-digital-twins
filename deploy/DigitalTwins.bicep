param location string
param baseName string

resource digitalTwinsInstance 'Microsoft.DigitalTwins/digitalTwinsInstances@2020-12-01' = {
  name: '${replace(baseName, '-', '')}adt'
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

output id string = digitalTwinsInstance.id
output endpoint string = digitalTwinsInstance.properties.hostName


output deploymentOutputs object = {
  adt: {
    id: digitalTwinsInstance.id
    endpointUrl: digitalTwinsInstance.properties.hostName
  }
}
