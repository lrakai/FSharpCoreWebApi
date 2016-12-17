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

    let documentPath docType (id : string) = 
        (DocumentPath<'T>.op_Implicit id).Index(index).Type(docType)

    let indexItem item = 
        client.Index<'T>(item, func (indexer index))

    let getResult id = 
        client.Get(documentPath (TypeName.From<'T>()) (id |> string))
        
    let getAllItems = 
        Seq.cast(client.Search<'T>().Documents)

    let searchRequest (property : string) (keyword : string) = 
        new SearchRequest<'T>(From = Nullable<int>(0), Size = Nullable<int>(50), 
            Query = new QueryContainer(new QueryStringQuery(Query = keyword)))
            
    let searchItems property keyword = 
        let request = searchRequest property keyword
        Seq.cast(client.Search<'T>(request).Documents)
        
    let deleteItem id = 
        client.Delete(documentPath (TypeName.From<'T>()) (id |> string))

    interface ISearchableRepository<'T> with
        member self.GetAll () = 
             getAllItems

        member self.Get (id : Guid) = 
            let result = getResult id
            match result.Found with
            | false -> None
            | true -> Some(result.Source)

        member self.Search property keyword =
            searchItems property keyword
                        
        member self.Add (item : 'T) =
            item.Id <- Guid.NewGuid()
            ((self :> ISearchableRepository<'T>).Update item).Value
            
        member self.Update (item : 'T) =
            let indexResponse = indexItem item
            Some(item)

        member self.Remove (id : Guid) =
            let foundItem = ((self :> ISearchableRepository<'T>).Get id)
            match foundItem with
            | Some value -> deleteItem id |> ignore
            | _ -> ()
            foundItem
