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
cd src/FSharpWebApi && dotnet restore && dotnet run
```
When finished the server will be listening on port 5000.

## Test
```
dotnet restore && cd test/FSharpWebApi.Tests && dotnet test
```

## Dockerizing
Two containers are used, one for building and one for deploying the app, to reduce the size of the deployed image.

### Step 1: Build container
```
docker build -t fsharpwebapi-build -f Dockerfile.build .
docker create --name fsharpwebapi-build fsharpwebapi-build
docker cp fsharpwebapi-build:/out ./src/FSharpWebApi/bin/Publish
```

### Step 2: App container
```
docker build -t fsharpwebapi .
docker run -d -p 80:5000 -e "FSHARPWEBAPI_ES_CONNECTION=http://elasticsearch_server:9200" fsharpwebapi
```