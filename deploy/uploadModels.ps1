$dtEndpointUrl = "fmdevwestus2adt.api.wus2.digitaltwins.azure.net"

$modelsDir = Join-Path -Path $pwd -ChildPath "models"

az dt model create `
	--dt-name $dtEndpointUrl `
	--from-directory $modelsDir `
	--failure-policy Rollback