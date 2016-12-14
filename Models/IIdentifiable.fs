namespace FSharpWebApi.Models

open System

type IIdentifiable =
    abstract member Id : int with get, set