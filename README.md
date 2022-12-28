# Azure Digital Twins Application Sample

## Create Azure resources

1. Open `deploy\deploy.ps1` and change the deployment location.
2. Open `deploy\parameters.dev.json` and change the value of the `solutionName` parameter to a unique string.
3. Save the file.
4. Open a Powershell session and go to the repository root.
5. Run `deploy\deploy.ps1` which starts the deployment.
6. When the deployment completes, copy the `outputs` JSON object as you will need these values later.
7. Verify the resource group was created.
8. Verify an Azure Digital Twins service instance is in the resource group.

## Setup Access to Azure Digital Twins Explorer

### Create Organizational User Account

Personal accounts cannot access Azure Digital Twins Explorer. Therefore, you must create an organizational account in your Active Directory tenant.

1. Open Azure Active Directory.
2. Click the **Users** tab.
3. Click **New User** -> **Create New User**.
4. Enter values for the following fields:
   - User name
   - Name
5. Click **Let me create a password** and enter a temporary password. You will be asked to change this when logging in for the first time.
6. Click **Create**.
7. After a minute or so, go back to the **Users** section and click **All Users**. You should see the new account in the list. Copy the user principal name for this account as you will need it in the next section.

### Add Organizational User Account

1. In the Azure portal, go to the Digital Twins service instance created earlier.
2. Click **Access Control (IAM)**.
3. Click **Add role assignment**.
4. Click **Azure Digital Twins Data Owner** so that it is highlighted.
5. Clickn the **Members** tab.
6. Click **Select members**.
7. In the select box, paste the user prinicpal name of the user you created earlier.
8. Click the account name so that it is highlighted.
9. Click the **Select** button.
10. Click **Review + assign**.
11. Click **Review + assign** once more.

## Login to Azure Digital Twins Explorer

Before proceeding, disable your browser's popup blocker or better yet allow popups from **explorer.azuredigitaltwins.net**.

1. In the Azure portal, go to the Digital Twins service instance created earlier.
2. Click **Overview**.
3. Click **Open Azure Digital Twins Explorer (preview)**.
4. Login using the account you created earlier.
5. Change the password when propmpted.
6. If the login was successful, you should see an empty window like the one shown [here](https://learn.microsoft.com/en-us/azure/digital-twins/concepts-azure-digital-twins-explorer).

If you click **Run Query** no results will be returned because you haven't created any digital twin instances. Click **Models** and observe no models are present because you haven't uploaded any models.

## Upload Digital Twin Models

1. Open `deploy\uploadModels.ps1` and specify the Azure Digital Twin Service's endpoint url for `$dtEndpointUrl`. The URL can be found in the deployment output you copied earlier.
2. Save the file.
3. Open a Powershell session and go to the repository root.
4. Type `az login` and login using the **organizational account** you created earlier. The organizational account is a member of the Azure Digital Twins Data Owner role and will have permissions to upload models. Don't use your personal Azure account as it doesn't have sufficient permissions.
5. Run `deploy\uploadModels.ps1`.
6. Open the Azure Digital Twins Explorer.
7. Click the **Models** tab on the left.
8. Verify that the three models - **Wine Bottle**, **Wine Rack**, and **Wine Rack Slot** are present in the list.
9. Click the **Model Graph** tab on the right.
10. Verify the following relationships are present:

- Wine Rack Slot **partOf** Wine Rack
- Wine Rack **ownedBy** Organization
- Scanner **attachedTo** Wine Rack
- Wine Bottle **storedIn** Wine Rack Slot

## Build the Docker Containers

The solution contains two projects - a console app representing the wine rack device, and an Azure Function app that processes messages from the wine rack. These projects can be built as Docker containers, simplifying their execution.

1. Open a Powershell session and go to the `src\docker` folder.
2. Run `docker-compose pull`
3. Run `docker-compose build`
4. Run `docker image ls` and verify the following images are listed:
   - docker-winerack
   - docker-messageprocessor

## Onboard the Wine Rack Device

The process below simulates the onboarding of a new wine rack. A device is created in the IoT Hub. This device will have its own device connection string to the IoT Hub.

1. Open a Powershell session and go to the `src\docker` folder.
2. Run `docker run -it --entrypoint /bin/bash docker-winerack`
3. At the bash prompt, run `dotnet ./swr.dll`. You will get an error because not command line options were specified but at least you know the app is working.

### Create a IoT Hub Virtual Device for the Wine Rack

1. Open a new Powershell session and go to the repository root.
2. Run `az iot hub connection-string show --hub-name [Your IoT Hub host name] --output tsv`. The IoT Hub host name can be found in the deployment output you copied earlier.
3. Copy the connection string.
4. Go back to the bash prompt and run `dotnet ./swr.dll onboard iothub "[Your IoT Hub connection string]" testwinerack`. Make sure the connection string is enclosed in double-quotes.
5. Now get the wine rack's dedicated IoT Hub connection string by running `az iot hub device-identity connection-string show --hub-name [Your IoT Hub host name] --device-id testwinerack --output tsv` in the Azure CLI Powershell session.
6. Copy the connection string in the output as you will need it later.

### Configure the Wine Rack's With the Dedicated IoT Hub Connection String

In the bash prompt, run `dotnet ./swr.dll config add IotHubConnectionString "[Your wine rack's connection string]"`. This is the connection string you copied in the previous step. Make sure the connection string is enclosed in double-quotes.

The wine rack will send IoT Hub messages using this connection string.

### Onboard the Wine Rack to the Azure Digital Twins Service

In this step, you will have the Wine Rack create a message that instructs the Azure Digital Twin to create the Wine Rack's twin.

In the bash prompt, run `dotnet ./swr.dll onboard twin "My Org" 4`. The first argument is the name of the organization that owns the wine rack. The second parameter is the number of wine rack slots.

## Process the Onboarding Message

At this point the wine rack has sent a message to the IoT Hub containing the new wine rack's details. To process this message and create the digital twin, run the Function App.

## Cleanup

### Azure cleanup

#### Delete Resource Group

Delete your resource group by running the following in a Powershell session:

```powershell
az group delete --name [YOUR RESOURCE GROUP NAME] --yes
```

#### Delete Organizational Account

Delete the account you created to access the Azure Digital Twins Explorer.

1. Open Azure Active Directory.
2. Click the **Users** tab.
3. Click the checkbox next to the user.
4. Click the **Delete** button.
