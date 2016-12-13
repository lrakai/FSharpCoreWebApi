namespace FSharpWebApi.Models

open System
open System.ComponentModel.DataAnnotations

type Todo() = 
    member val Id = 0 with get, set
    interface IIdentifiable with
        [<Key>] member self.Id with get () = self.Id 
    
    [<Required>] member val Message = "" with get, set

