namespace FSharpWebApi.Repository

open System
open System.Linq
open Microsoft.FSharp.Collections
open FSharpWebApi.Models
open Nest

type ElasticsearchConfiguration(indexName : string) =
    member val IndexName = indexName

// Some NEST snippets from https://gist.github.com/mjul/f5c936d116a2b89c3da8f63afad628e7/
type ElasticsearchRepository<'T when 'T :> IIdentifiable and 'T : not struct>(configuration : ElasticsearchConfiguration) =
    let node = new Uri("http://127.0.0.1:9200")
    let settings = (new ConnectionSettings(node)).DefaultIndex(configuration.IndexName)
    let client = new ElasticClient(settings)
    
    let indexName (name:string) =
        IndexName.op_Implicit name

    let index = indexName configuration.IndexName 

    let indexer index (ides:IndexDescriptor<'a>)  =
        ides.Index(index) :> IIndexRequest

    // Convert F# functions to System.Func<_,_> types
    // for interop with the Elasticsearch NEST API
    let func (f:('a->'b)) =
        new Func<'a,'b>(f)

    let documentPath docType (id:string) = 
        (DocumentPath<'T>.op_Implicit id).Index(index).Type(docType)

    let indexItem item = 
        client.Index<'T>(item, func (indexer index))

    let getResult id = 
        client.Get(documentPath (TypeName.From<'T>()) (id |> string))

    let searchItems = 
        Seq.cast(client.Search<'T>().Documents)
        
    let deleteItem id = 
        client.Delete(documentPath (TypeName.From<'T>()) (id |> string))

    interface FSharpWebApi.Repository.IRepository<'T> with
        member x.GetAll () = 
             searchItems

        member x.Get (id : Guid) = 
            let result = getResult id
            match result.Found with
            | false -> None
            | true -> Some(result.Source)
                        
        member x.Add (item : 'T) =
            item.Id <- Guid.NewGuid()
            ((x :> FSharpWebApi.Repository.IRepository<'T>).Update item).Value
            
        member x.Update (item : 'T) =
            let indexResponse = indexItem item
            Some(item)

        member x.Remove (id : Guid) =
            let foundItem = ((x :> FSharpWebApi.Repository.IRepository<'T>).Get id)
            match foundItem with
            | Some value -> deleteItem id |> ignore
            | _ -> ()
            foundItem
