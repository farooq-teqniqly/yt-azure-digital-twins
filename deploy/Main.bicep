targetScope = 'subscription'

param solutionName string

@allowed([
  'dev'
  'test'
])
param environmentName string

var location = deployment().location
var baseName = '${solutionName}-${environmentName}-${location}'
var rgName = '${baseName}-rg'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: rgName
  location: location
}

module digitalTwinsDeploy 'DigitalTwins.bicep' = {
  name: 'digitalTwinsDeploy'
  scope: rg
  params: {
    baseName: baseName
    location: location
  }
}

module iotHubDeploy 'IoTHub.bicep' = {
  name: 'iotHubDeploy'
  scope: rg
  params: {
    baseName: baseName
    location: location
  }
}

output deploymentOutputs object = {
  resourceGroupName: rgName
  digitalTwinsDeployment: {
    endpointUrl: digitalTwinsDeploy.outputs.deploymentOutputs.adt.endpointUrl
  }
  iotHubDeployment: {
    hostName: iotHubDeploy.outputs.deploymentOutputs.iotHub.hostName
    eventHubEndpointUrl: iotHubDeploy.outputs.deploymentOutputs.iotHub.eventHubEndpointUrl
  }
}
