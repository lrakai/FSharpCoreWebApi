namespace FSharpWebApi.Repository

open System
open System.Linq
open System.Reflection
open Microsoft.FSharp.Collections
open FSharpWebApi.Models

type InMemoryRepository<'T when 'T :> IIdentifiable >() =
    static let mutable _list : List<'T> = []

    let findId id item = (item :> IIdentifiable).Id = id

    let (|InvariantEqual|_|) (keyword : string) (str : string) = 
        if str.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
        then Some() else None

    let stringMatch property keyword item =
        let propertyInfo = item.GetType().GetProperty(property)
        match (propertyInfo.GetValue(item, null) |> string) with
        | InvariantEqual keyword -> true
        | _ -> false 
    
    let replace index replacement = List.mapi (fun i item -> if i = index then replacement else item)
    
    let rec removeFirst predicate list =
        match list with
        | head::tail when predicate head -> tail
        | head::tail -> head::removeFirst predicate tail
        | _ -> []

    member self.clear () =
        _list <- []

    interface ISearchableRepository<'T> with
        member self.GetAll () = 
            lock _list (fun () -> 
                _list :> _)

        member self.Search property keyword =
            lock _list (fun () -> 
                List.filter (stringMatch property keyword) _list :> _)

        member self.Get (id : Guid) = 
            lock _list (fun () -> 
                List.tryFind (findId id) _list)
                
        member self.Add (item : 'T) =
            lock _list (fun () ->
                (item :> IIdentifiable).Id <- Guid.NewGuid()
                _list <- List.append _list [item]
                item)

        member self.Update (item : 'T) =
            lock _list (fun () ->
                let foundIndex = List.tryFindIndex (findId (item :> IIdentifiable).Id) _list
                match foundIndex with
                | None -> None
                | Some value ->
                    _list <- replace value item _list
                    Some item)

        member self.Remove (id : Guid) =
            lock _list (fun () ->
                let foundItem = List.tryFind (findId id) _list
                match foundItem with
                | None -> None
                | Some value ->
                    _list <- removeFirst (fun (item) -> (item :> IIdentifiable).Id = (value :> IIdentifiable).Id) _list
                    foundItem)
