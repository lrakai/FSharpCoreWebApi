﻿namespace FSharpWebApi.Controllers
 
open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Routing
open FSharpWebApi.Repository
open FSharpWebApi.Models
 
[<Route("api/todos")>]
type TodosController(repository : IRepository<Todo>) as self =
    inherit Controller()
    
    let optionResponse =
        function
        | Some todo -> self.Ok(todo) :> IActionResult
        | None -> self.NotFound() :> IActionResult

    [<HttpGet>]
    member x.Get() =
        repository.GetAll()

    [<HttpGet>]
    [<Route("{id:int}", Name = "GetById")>]
    member x.Get id : IActionResult =
        let todo = repository.Get id
        optionResponse todo

    
    [<HttpPost>]
    member x.Post ([<FromBody>] todo : Todo) : IActionResult =
        match x.ModelState.IsValid with 
        | false -> x.BadRequest(x.ModelState) :> _
        | true  -> 
            try
                let addedItem = repository.Add todo
                let rv = RouteValueDictionary()
                rv.Add("id", (todo :> IIdentifiable).Id)
                x.CreatedAtRoute("GetById", rv, addedItem) :> _
            with
                | _ -> x.StatusCode(500, "Operation failed") :> _

    [<HttpPut>]
    [<Route("{id:int}")>]
    member x.Put id ([<FromBody>] todo : Todo) : IActionResult =
        match x.ModelState.IsValid with
        | false -> x.BadRequest(x.ModelState) :> _
        | true -> match todo.Id = id with
                  | false -> x.BadRequest("id must match member Id property") :> _
                  | _ -> 
                        let result = repository.Update todo
                        x.Ok(result) :> _

    [<HttpDelete>]
    [<Route("{id:int}")>]
    member x.Delete id : IActionResult =
        let todo = repository.Remove id
        optionResponse todo
