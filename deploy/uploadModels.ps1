$dtEndpointUrl = "[Azure Digital Twin Service Endpoint URL]"

$modelsDir = Join-Path -Path $pwd -ChildPath "models"

az dt model create `
	--dt-name $dtEndpointUrl `
	--from-directory $modelsDir `
	--failure-policy Rollback