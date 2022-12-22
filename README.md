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
- Wine Bottle **storedIn** Wine Rack Slot

## Onboard the Wine Rack Device

The process below simulates the onboarding of a new wine rack. A device is created in the IoT Hub. This device will have its own device connection string to the IoT Hub.

1. Open `deploy\onboardDevice.ps1` and specify the Azure IoT Hub's hostname for `$iotHubName`. The hostname can be found in the deployment output you copied earlier. Specify your device's "serial number" for `$deviceName`.
2. Save the file.
3. Open a Powershell session and go to the repository root.
4. Run `deploy\onboardDevice.ps1`.
5. Copy the connection string in the output as you will need it later.

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
