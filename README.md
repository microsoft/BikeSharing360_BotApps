# BikeSharing360_BotApps
BikeSharing360Bot shows how you can help your business grow and scale its customer service abilities through an intelligent bot that can understand customers and know when it might need to involve humans to help more complex issues. 
This application integrates Language Understanding Intelligent Service (LUIS) to help understand customer intents which then launched into appropriate business flows and promptes users with appropriate and succinct questions to gather details, communicated with employees aggregated data, and could stay in touch with customers instantly when it needs to communicate timely updates and other information.  

## Screens

<img src="images/bot1.png" Width="210" />
<img src="images/bot2.png" Width="210" />
<img src="images/bot3.png" Width="210" />
<img src="images/bot4.png" Width="210" />

## Requirements
You need an Azure subscription to deploy the bot service. [Try it for free](https://azure.microsoft.com/en-us/) 

## Setup the bot service
* 1. Deploy CustomerServiceApis into Azure (You need a bing map service subscription to make "GetMapWithRoute" api work).
* 2. Create a new Azure language understanding bot service. [How](https://docs.botframework.com/en-us/azure-bots/build/first-bot/#navtitle)
* 3. Setup continuous integration. [How](https://docs.botframework.com/en-us/azure-bot-service/manage/setting-up-continuous-integration/#navtitle)
* 4. Submit the BikeSharing360Bot code into repository.
* 5. Sign into https://www.luis.ai and find the application created in step 2.
* 6. Click "New App"->"Import Existing Application" and choose luis\BikeSharing360Luis.json. BikeSharing360Luis will be created.
* 7. Enter the "BikeSharing360Luis" app. Click "train" button on the left bottom. Then click "publish" on the left. Save the app-id and subscription-key information from the URL.
* 8. Go back to the code. Open BikeSharing360LuisDialog.csx. Replace "_YourModelId_", "_YourSubscriptionKey_" with the keys you got in step 7.
* 9. Submit the code.
* 10 Now, you can publish the bot ([How](https://docs.botframework.com/en-us/azure-bot-service/manage/publish/#navtitle)) and test it. 
