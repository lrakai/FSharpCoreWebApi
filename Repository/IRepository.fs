namespace FSharpWebApi.Repository

open System
open FSharpWebApi.Models

type IRepository<'T when 'T :> IIdentifiable > = 
    abstract member GetAll : unit -> seq<'T>
    abstract member Get : Guid -> 'T option
    abstract member Add : 'T -> 'T
    abstract member Update : 'T -> 'T option
    abstract member Remove : Guid -> 'T option

