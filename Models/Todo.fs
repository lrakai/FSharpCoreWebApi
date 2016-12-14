namespace FSharpWebApi.Models

open System
open System.ComponentModel.DataAnnotations

type Todo() = 
    interface IIdentifiable with
        member self.Id with get () = self.Id and set value = self.Id <- value
    
    [<Key>] member val Id = 0 with get, set
    [<Required>] member val Message = "" with get, set

