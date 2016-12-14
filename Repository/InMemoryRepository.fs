namespace FSharpWebApi.Repository

open System.Linq
open Microsoft.FSharp.Collections
open FSharpWebApi.Models

type InMemoryRepository<'T when 'T :> IIdentifiable >() =
    static let mutable _list : List<'T> = []
    static let _index = ref 0

    let findId id item = (item :> IIdentifiable).Id = id
    
    let rec removeFirst predicate list =
        match list with
        | head::tail when predicate head -> tail
        | head::tail -> head::removeFirst predicate tail
        | _ -> []

    interface IRepository<'T> with
        member x.GetAll () = 
            lock _list (fun () -> 
                _list :> _)

        member x.Get (id : int) = 
            lock _list (fun () -> 
                List.tryFind (findId id) _list)
                
        member x.Add (item : 'T) =
            lock _list (fun () ->
                (item :> IIdentifiable).Id <- _index.Value
                _list <- List.append _list [item]
                incr _index
                item)

        member x.Remove (id : int) =
            lock _list (fun () ->
                let foundItem = List.tryFind (findId id) _list
                match foundItem with
                | None -> None
                | Some v ->
                    _list <- removeFirst (fun (item) -> (item :> IIdentifiable).Id = (v :> IIdentifiable).Id) _list
                    foundItem)
