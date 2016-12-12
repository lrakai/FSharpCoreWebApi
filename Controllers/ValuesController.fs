namespace FSharpWebApi.Controllers
 
open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
 
[<Route("api/Values")>]
type ValuesController() =
    inherit Controller()
 
    [<HttpGet>]
    member this.Get() =
        ["x"; "z"]