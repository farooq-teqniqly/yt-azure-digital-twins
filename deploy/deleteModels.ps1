$dtEndpointUrl = "fmdevwestus2adt.api.wus2.digitaltwins.azure.net"

$modelsDir = Join-Path -Path $pwd -ChildPath "models"

$fileNames = Get-ChildItem $modelsDir

foreach ($fileName in $fileNames) {
    $json = Get-Content $fileName -Raw | ConvertFrom-Json
    $dtmi = $json."@id"

    az dt model delete `
	    --dt-name $dtEndpointUrl `
	    --dtmi $dtmi `
}