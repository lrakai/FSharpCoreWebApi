namespace FSharpWebApi.Controllers
 
open System
open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Routing
open FSharpWebApi.Repository
open FSharpWebApi.Models
 
[<Route("api/todos")>]
type TodosController(repository : ISearchableRepository<Todo>) as self =
    inherit Controller()
    
    let optionResponse =
        function
        | Some todo -> self.Ok(todo) :> IActionResult
        | None -> self.NotFound() :> IActionResult

    [<HttpGet>]
    member self.Get (search : string) : IActionResult =
        match String.IsNullOrEmpty(search) with
        | true -> self.Ok(repository.GetAll()) :> _
        | false -> self.Ok(repository.Search "Message" search) :> _

    [<HttpGet>]
    [<Route("{id:guid}", Name = "GetById")>]
    member self.Get id : IActionResult =
        let todo = repository.Get id
        optionResponse todo
    
    [<HttpPost>]
    member self.Post ([<FromBody>] todo : Todo) : IActionResult =
        match self.ModelState.IsValid with 
        | false -> self.BadRequest(self.ModelState) :> _
        | true  -> 
            try
                let addedItem = repository.Add todo
                let rv = RouteValueDictionary()
                rv.Add("id", (todo :> IIdentifiable).Id)
                self.CreatedAtRoute("GetById", rv, addedItem) :> _
            with
                | _ -> self.StatusCode(500, "Operation failed") :> _

    [<HttpPut>]
    [<Route("{id:guid}")>]
    member self.Put id ([<FromBody>] todo : Todo) : IActionResult =
        match self.ModelState.IsValid with
        | false -> self.BadRequest(self.ModelState) :> _
        | true -> match todo.Id = id with
                  | false -> self.BadRequest("id must match member Id property") :> _
                  | _ -> 
                        let result = repository.Update todo
                        self.Ok(result) :> _

    [<HttpDelete>]
    [<Route("{id:guid}")>]
    member self.Delete id : IActionResult =
        let todo = repository.Remove id
        optionResponse todo
