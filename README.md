# Giovanni
## TASK 1

1. Run before ```AzureStorageEmulator.exe start```
2. Start ```Giovanni.Task1``` Azure function locally to test functionality.

Exposed HTTP endpoints:

- ```http://localhost:7060/api/publicapi/log?from=<DateTimeOffset>&to=<DateTimeOffset>```
eg. http://localhost:7060/api/publicapi/log?from=2023-03-27T18:46:21.6724703%2B02:00&to=2023-03-28T20:18:23.6724703%2B02:00

- ```http://localhost:7060/api/publicapi/payload/{blobName}```
eg. http://localhost:7060/api/publicapi/payload/d1f54980-2b97-4c47-a466-10e553e03656_20230328180100.json

Task 1 contains Azure function with 3 functions inside named:

- PublicApiFetchFunction
- PublicApiFetchFunction-HttpGetLogs
- PublicApiFetchFunction-HttpGetPayload

## TASK 2

1. Start backend in Debug mode - ```Giovanni.Task2 - C# ASP CORE 6``` (edit your SqlServer connection in ```appsettings.Development.json```, migrations and database will be created by EF CORE).
2. Navigate to ```./Task2/WebApp/giovanniweather``` and type ```npm install``` then ```npm start``` to start React app.
3. If you notice CORS problem, install ```https://chrome.google.com/webstore/detail/allow-cors-access-control/lhobafahddgcelffkeicbaginigeejlf?hl=en``` extension to Your browser and follow the tutorial: ```https://mybrowseraddon.com/access-control-allow-origin.html?v=0.1.8&type=install```
3. WIP - Trends are not shown yet on click on bar.

## Thanks and Happy review !
