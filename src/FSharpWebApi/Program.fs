open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open FSharpWebApi.Models
open FSharpWebApi.Repository
 
type Startup(env: IHostingEnvironment) =
 
    member this.ConfigureServices(services: IServiceCollection) =
        let elasticsearchConfiguration = new ElasticsearchConfiguration("todos")
        services.AddSingleton<ElasticsearchConfiguration>(elasticsearchConfiguration) |> ignore
        services.AddScoped<ISearchableRepository<Todo>, ElasticsearchRepository<Todo>>() |> ignore
        services.AddMvc() |> ignore
 
    member this.Configure (app: IApplicationBuilder, loggerFactory: ILoggerFactory) =
        app.UseMvc() |> ignore
        loggerFactory.AddConsole() |> ignore
 
[<EntryPoint>]
let main argv = 
    printfn "Starting"
    let host = WebHostBuilder()
                .UseUrls("http://0.0.0.0:5000") // bind to any ipv4 address (or run with dotnet run --server.urls=http://0.0.0.0:5000)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
    host.Run()
    0 //exit code