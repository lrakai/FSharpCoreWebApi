open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
 
type Startup(env: IHostingEnvironment) =
 
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddMvc() |> ignore
 
    member this.Configure (app: IApplicationBuilder) =
        app.UseMvc() |> ignore
 
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