# Azure IoT Sample - Child for IoT Edge provisioning

This sample shows how you can assign leaf devices to an Edge parent by subscribing to DeviceCreated events in IoT Hub. We assume a parent device's name is used as suffix to the leaf device name with a dash (-) to separate the name (like so: `child1-parent1`).

## Pre-requisites
This sample assumes you have access to an Azure subscription and know how to use the Azure Portal and the CLI.
- Visual Studio Code
- Install the required tools for Azure Functions as described [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code) for developing your Azure Functions with Visual Studio Core.

## To setup the flow

1. Clone this repo. Navigate to the folder /dps-child-edge.
1. Open the folder in Visual Studio Code.
1. Create Azure resources
    - Create IoT Hub: https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-create-using-cli
    - Create DPS resource: `az iot dps create --name [yourchoiceofname] --resource-group [yourpreviouslycreatedrm] --location westeurope`
    - Create a Storage account, either via portal or in Cloud Shell: `az storage account create --name [yourchoice] --resource-group [yourname] --kind BlobStorage --location [yourchoice for example westeurope] --sku Standard_LRS --access-tier Hot`
    - Create an Azure Function in the same resource group, select your previously created Resource Group and the Azure Storage account.

1. Publish and configure Azure Function.
    - From Visual Studio Code, if you have all pre-requisites installed can publish the Function via the Azure Functions Core Tools. Please see here: https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code#publish-the-project-to-azure
    - Add a new Application Variable to the Function app: 
        - Go to your IoT Hub > Access Keys > registryReadWrite: copy this connection string.
        - Now go to your Azure Function > Configuration > Application Settings > New application setting. Name it `IoTHubConnectionString` and paste the previously copied connection string in the value field. Click Update. Click SAVE at the top.

1. Configure the Event Grid Registration
    - In the Azure Function, click the `EventGridChildCreated` function and there choose Add Event Grid subscription. Provide a new name and selec the IoT Hub you created. Make sure you only choose 'DeviceCreated' as the Filter to Event types and then click Create.

1. Test your function: 
    - Create a new IoT Edge device via the Portal. Name the device `parent1`.
    - Create a new normal device, name the device `child1-parent1`.
    - Either review the logs for the Function, or go to the `parent1` edge device and validate the leaf device has been assigned.

At this stage you have configured the basic setup to have your leaf devices to get automatically assigned to a parent IoT Edge device. Note the naming of your devices is supposed to be by convention. 

The next phase will be on completing the setup with IoT Edge gateway device, a client device and DPS. This will be added to the project at a later stage.


Please note this is provided as a sample to build from, not production-ready code.

