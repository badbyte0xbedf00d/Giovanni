# Giovanni
## TASK 1

1. Run before ```AzureStorageEmulator.exe start```
2. Start ```Giovanni.Task1``` Azure function locally to test functionality.

Exposed HTTP endpoints:

- ```http://localhost:7060/api/publicapi/log?from=<DateTimeOffset>&to=<DateTimeOffset>
eg. http://localhost:7060/api/publicapi/log?from=2023-03-27T18:46:21.6724703%2B02:00&to=2023-03-28T20:18:23.6724703%2B02:00

- ```http://localhost:7060/api/publicapi/payload/{blobName}```
eg. http://localhost:7060/api/publicapi/payload/d1f54980-2b97-4c47-a466-10e553e03656_20230328180100.json

Task 1 contains Azure function with 3 functions inside named:

- PublicApiFetchFunction
- PublicApiFetchFunction-HttpGetLogs
- PublicApiFetchFunction-HttpGetPayload

## Thanks and Happy review !
