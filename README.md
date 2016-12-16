# FSharpCoreWebApi
ASP.NET Core todo note Web API in F#.  Includes in-memory and Elasticsearch repositories.

## API
```
Method Path            Body
GET    /
GET    ?search=keyword 
GET    /{id:guid}      
POST   /               {"Message": "Todo message"}
PUT    /{id:guid}      {"Id": id, "Message": "New todo message"}
DELETE /{id:guid}
```

## Run
```
dotnet restore && dotnet run
```
When finished the server will be listening on port 5000.
