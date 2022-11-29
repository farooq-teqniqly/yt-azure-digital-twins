# Azure Digital Twins Application Sample

## Create Azure resources

1. Open `deploy\deploy.ps1` and change the deployment location.
2. Open `deploy\parameters.dev.json` and change the value of the `solutionName` parameter to a unique string.
3. Save the file.
4. Open a Powershell session and go to the repository root.
5. Run `deploy\deploy.ps1` which starts the deployment.
6. Verify the resource group was created.
7. Verify an Azure Digital Twins service instance is in the resource group.

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
