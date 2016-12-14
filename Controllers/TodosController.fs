namespace FSharpWebApi.Controllers
 
open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Routing
open FSharpWebApi.Repository
open FSharpWebApi.Models
 
[<Route("api/todos")>]
type TodosController(fromRepository : IRepository<Todo>) as self =
    inherit Controller()
    
    let optionResponse =
        function
        | Some todo -> self.Ok(todo) :> IActionResult
        | None -> self.NotFound() :> IActionResult

    [<HttpGet>]
    member x.Get() =
        fromRepository.GetAll()

    [<HttpGet>]
    [<Route("{id:int}", Name = "GetById")>]
    member x.Get id : IActionResult =
        let todo = fromRepository.Get id
        optionResponse todo

    
    [<HttpPost>]
    member x.Post ([<FromBody>] todo : Todo) : IActionResult =
        match x.ModelState.IsValid with 
        | false -> x.BadRequest(x.ModelState) :> _
        | true  -> 
            try
                let addedItem = fromRepository.Add todo
                let rv = RouteValueDictionary()
                rv.Add("id", (todo :> IIdentifiable).Id)
                x.CreatedAtRoute("GetById", rv, addedItem) :> _
            with
                | _ -> x.StatusCode(500, "Operation failed") :> _

    [<HttpDelete>]
    [<Route("{id:int}")>]
    member x.Delete id : IActionResult =
        let todo = fromRepository.Remove id
        optionResponse todo
