$iotHubName = "[Your IoT Hub name]"
$deviceName = "[Your device's serial code]"

az iot hub device-identity create `
    --hub-name $iotHubName `
    --device-id $deviceName

az iot hub device-identity connection-string show `
    --hub-name $iotHubName `
    --device-id $deviceName