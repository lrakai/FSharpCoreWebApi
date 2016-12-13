namespace FSharpWebApi.Repository

open System.Linq
open FSharpWebApi.Models

type InMemoryRepository<'T when 'T :> IIdentifiable >() =
    let mutable _list : List<'T> = []
    
    interface IRepository<'T> with
        member x.GetAll () = 
            lock _list (fun () -> 
                _list :> _)

        member x.Get (id : int) = 
            let findId id todo = (todo :> IIdentifiable).Id = id
            lock _list (fun () -> 
                List.find (findId id) _list)
                

