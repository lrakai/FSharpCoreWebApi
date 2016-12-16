namespace FSharpWebApi.Repository

open FSharpWebApi.Models

type ISearchableRepository<'T when 'T :> IIdentifiable > =
    inherit IRepository<'T>
    
    abstract member Search : string -> string -> seq<'T>
    