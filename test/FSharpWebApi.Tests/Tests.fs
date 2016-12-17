namespace FSharpWebApi.Tests

open System
open Microsoft.AspNetCore.Mvc
open Xunit
open FSharpWebApi.Models
open FSharpWebApi.Controllers
open FSharpWebApi.Repository

module FSharpWebApiTests =

    let getRepository() =
        let repository = new InMemoryRepository<Todo>()
        repository.clear()
        (repository :> IRepository<Todo>).Add (new Todo(Message = "Hello")) |> ignore
        (repository :> IRepository<Todo>).Add (new Todo(Message = "World")) |> ignore
        repository

    let getController() =
        new TodosController(getRepository())

    let getTodos (controller : TodosController) (search : string) =
        ((controller.Get search) :?> OkObjectResult).Value :?> seq<Todo>

    [<Fact>]
    let ``Get without search returns all with Ok response``() =
        use todosController = getController()

        let result = todosController.Get ""

        Assert.True(result :? OkObjectResult)
        let todos = (result :?> OkObjectResult).Value :?> seq<Todo>
        Assert.False(Seq.isEmpty todos)
        Assert.True(Seq.length todos = 2)

    [<Fact>]
    let ``Get with search returns some with Ok response``() =
        use todosController = getController()

        let result = todosController.Get "World"

        Assert.True(result :? OkObjectResult)
        let todos = (result :?> OkObjectResult).Value :?> seq<Todo>
        Assert.False(Seq.isEmpty todos)
        Assert.True(Seq.length todos = 1)

    [<Fact>]
    let ``Get id returns correct todo with Ok response``() =
        use todosController = getController()
        let worldTodo = getTodos todosController "World" |> Seq.head

        let result = todosController.Get worldTodo.Id

        Assert.True(result :? OkObjectResult)
        let foundTodo = (result :?> OkObjectResult).Value :?> Todo
        Assert.True(foundTodo.Id = worldTodo.Id)

    [<Fact>]
    let ``Get unknown id has NotFound response``() =
        use todosController = getController()

        let result = todosController.Get Guid.Empty

        Assert.True(result :? NotFoundResult)

    [<Fact>]
    let ``Post adds with Created response``() =
        use todosController = getController()
        let todo = new Todo(Message = "F#")

        let result = todosController.Post todo

        Assert.True(result :? CreatedAtRouteResult)
        let todos = getTodos todosController ""
        Assert.False(Seq.isEmpty todos)
        Assert.True(Seq.length todos = 3)

    [<Fact>]
    let ``Put updates with Ok response``() =
        use todosController = getController()
        let worldTodo = getTodos todosController "World" |> Seq.head
        worldTodo.Message <- "F#"

        let result = todosController.Put worldTodo.Id worldTodo

        Assert.True(result :? OkObjectResult)
        let fsTodos = getTodos todosController "F#"
        let worldTodos = getTodos todosController "World"
        Assert.False(Seq.isEmpty fsTodos)
        Assert.True(Seq.isEmpty worldTodos)
        Assert.True(Seq.length fsTodos = 1)

    [<Fact>]
    let ``Delete removes with Ok response``() =
        use todosController = getController()
        let todo = getTodos todosController "" |> Seq.head
        
        let result = todosController.Delete todo.Id

        Assert.True(result :? OkObjectResult)
        let todos = getTodos todosController ""
        Assert.False(Seq.isEmpty todos)
        Assert.True(Seq.length todos = 1)

    [<Fact>]
    let ``Delete unknown id has NotFound response``() =
        use todosController = getController()
        
        let result = todosController.Delete Guid.Empty

        Assert.True(result :? NotFoundResult)
