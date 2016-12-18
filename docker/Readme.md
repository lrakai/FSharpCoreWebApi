# FSharpCoreWebApi
ASP.NET Core todo note Web API in F# using an Elasticsearch repository.

# Configuration
The container exposes port 5000 and the Elasticsearch server URL is configured by the FSHARPWEBAPI_ES_CONNECTION environment variable.  
```
docker run -d -p 80:5000 -e "FSHARPWEBAPI_ES_CONNECTION=http://elasticsearch_server:9200" lrakai/fsharpcorewebapi
```
