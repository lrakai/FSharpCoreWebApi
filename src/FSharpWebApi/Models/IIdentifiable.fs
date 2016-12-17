namespace FSharpWebApi.Models

open System

type IIdentifiable =
    abstract member Id : Guid with get, set