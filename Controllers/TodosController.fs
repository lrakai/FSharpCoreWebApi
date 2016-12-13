namespace FSharpWebApi.Controllers
 
open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open FSharpWebApi.Repository
open FSharpWebApi.Models
 
[<Route("api/todos")>]
type TodosController(fromRepository : IRepository<Todo>) =
    inherit Controller()
    
    [<HttpGet>]
    member x.Get() =
        fromRepository.GetAll()

    [<Route("{id:int}")>]
    member x.Get id : IActionResult =
        try 
            let todo = fromRepository.Get id
            x.Ok(todo) :> _
        with
            | :? KeyNotFoundException -> x.BadRequest() :> _
