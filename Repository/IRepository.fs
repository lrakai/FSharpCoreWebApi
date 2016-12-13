namespace FSharpWebApi.Repository

open System
open System.Collections.Generic
open FSharpWebApi.Models

type IRepository<'T when 'T :> IIdentifiable > = 
    abstract member GetAll : unit -> seq<'T>
    abstract member Get : int -> 'T

