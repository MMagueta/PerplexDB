﻿namespace IO

module Utilities = begin
  [<RequireQualifiedAccess>]
  module String = begin
    let toBytes: string -> byte array =
        Array.ofSeq >> Array.collect (System.BitConverter.GetBytes >> Array.take 1)
  end 
end

module Write = begin
  open Language
  open Utilities

  type Fact = Map<string, Value.t>

  let convertLiteral (relationAttributes: Map<string, Entity.FieldMetadata>) (attributeName: string) (literal: Value.t): int32 * byte array =
    match literal, Map.tryFind attributeName relationAttributes with
    | Value.VVariableString elem, Some {fieldPosition = fieldPosition; type' = Type.TVariableString specificationSize} ->
       let serializedString = System.Text.Encoding.UTF8.GetBytes elem in
       /// Filling with the rest of the string specified size
       let serializedValue =
           Array.append
               serializedString
               [| for _ in 1..(specificationSize - serializedString.Length) do 0uy |]
       (fieldPosition, serializedValue)
    | Value.VInteger32 elem, Some {fieldPosition = fieldPosition; type' = Type.TInteger32} ->
       let serializedValue = System.BitConverter.GetBytes elem
       (fieldPosition, serializedValue)
    | _, Some _ -> failwith <| Printf.sprintf "Type mismatch for attribute '%s'." attributeName
    | _, None -> failwith <| Printf.sprintf "Attribute '%s' is not part of the relation." attributeName

  let convertFact (schema: Schema.t) (relationName: string) (fact: Fact) =
    match Map.tryFind relationName schema with
    | Some (Entity.Relation (relationAttributes, _)) ->
       Map.fold
         (fun acc key value -> Array.append [| convertLiteral relationAttributes key value |] acc)
         [||]
         fact
       |> Array.sortBy fst
       |> Array.map snd
       |> Array.concat
    | None -> failwithf "Relation '%s' could not be located in the working schema." relationName

  module Disk = begin
    open System
    open System.IO

    let rec lockedStream path attempts =
        if attempts > 0 then
            let mutable stream: System.IO.FileStream = null
            try stream <- new System.IO.FileStream(path, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite); Ok stream
            with :? IOException ->
                if isNull stream then Async.Sleep 100 |> Async.RunSynchronously; lockedStream path (attempts - 1)
                else Error ""
         else Error ""

    // Stream needs to live through the scope of the callee for now
    let writeFact stream (schema: Schema.t) (relationName: string) position (fact: Fact): unit =
        match Map.tryFind relationName schema with
        | Some (Entity.Relation (relationAttributes, physicalOffset)) ->
            let content = convertFact schema relationName fact
            let relationDefinitionSize = Schema.relationSize relationAttributes in
            // let path = "/tmp/perplexdb/" + relationName + ".ndf" in
            // Stuck with FileShare because of Lock being not available for mac
            // Replace this later with regional locking instead, right now locks the whole file
            // use stream = new IO.FileStream(path, IO.FileMode.OpenOrCreate, IO.FileAccess.Write, IO.FileShare.Read) in
            let binaryStream = IO.BinaryWriter(stream) in
            // logger.ForContext("ExecutionContext", "Write").Debug($"Position: {position}")
            let offset =
                match position with
                | None -> physicalOffset * relationDefinitionSize
                | Some offset -> offset
            let _ = binaryStream.Seek(offset, SeekOrigin.Begin)
            // Fill with blanks
            binaryStream.Write [| for _ in 1..relationDefinitionSize do 0uy |]
            let _ = binaryStream.Seek(offset, SeekOrigin.Begin)
            binaryStream.Write content
            binaryStream.Flush()
            //binaryStream.Dispose()
        | _ -> ()

  end  
end

module Read = begin
  open Language

  open System
  open System.Runtime.InteropServices

  type ColumnMap = Map<string, Value.t>
  
  let deserialize (schema: Schema.t) (entityName: string) (stream: byte array): ColumnMap =
    match Map.tryFind entityName schema with
    | Some (Entity.Relation (relationAttributes, _)) ->
        let columns  =
          Map.toList relationAttributes
          |> List.sortBy (fun (_, (x: Entity.FieldMetadata)) -> x.fieldPosition)
          |> List.map (fun (name, {type' = type';}) -> (type', name))
        in
        
        let rec reconstruct_columns acc stream = function
          | [] -> acc
          | [(type', name)] ->
             (name, Value.t.FromBytes type' stream) :: acc
          | (type', name)::typesRest ->
             let ret = (name, Value.t.FromBytes type' stream.[0..(type'.ByteSize - 1)]) :: acc in
             reconstruct_columns ret (stream.[type'.ByteSize..]) typesRest
        in reconstruct_columns [] stream columns |> Map.ofList
    | None -> failwithf "Entity '%s' could not be located in the working schema." entityName


  [<RequireQualifiedAccessAttribute>]
  module Tree =
      [<DllImport(__SOURCE_DIRECTORY__ + "/libbplustree.so", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)>]
      extern void print_leaves(void* _tree);
      [<DllImport(__SOURCE_DIRECTORY__ + "/libbplustree.so", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)>]
      extern void* insert(void* _tree, int _key, int _chunkNumber, int _pageNumber, int _slotNumber)
      [<DllImport(__SOURCE_DIRECTORY__ + "/libbplustree.so", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)>]
      extern void find_and_print(void * _tree, int _key, bool _verbose)
      [<DllImport(__SOURCE_DIRECTORY__ + "/libbplustree.so", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)>]
      extern void* find_and_get_value(void * _tree, int _key, bool _verbose)
      [<DllImport(__SOURCE_DIRECTORY__ + "/libbplustree.so", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)>]
      extern void* find_and_get_node(void * _tree, int _key)
  
  type ChunkNumber = int
  type PageNumber = int
  type InstanceNumber = int
  type OffsetNumber = int
  type Index =
      { chunkNumber: ChunkNumber
        pageNumber: PageNumber
        slotNumber: InstanceNumber
        entity: ColumnMap
        offset: int
        key: int }

  type Page = Index array array

  type IndexBuilder = OffsetNumber -> ChunkNumber -> PageNumber -> InstanceNumber -> ColumnMap -> Index

  let buildPages instancesPerPage (reader: IO.FileStream) instanceSize amountAlreadyRead chunkNumber (indexBuilder: IndexBuilder) schema entityName (relationToIndex: Map<string,Entity.FieldMetadata>) =
      let amountAlreadyRead = amountAlreadyRead
      let amountToRead =
          if instancesPerPage * instanceSize > ((reader.Length |> int32) - amountAlreadyRead)
          then ((reader.Length |> int32) - amountAlreadyRead)
          else instancesPerPage * instanceSize
      let mutable buffer = [|for _ in 1..amountToRead do 0uy|]
      reader.Read(buffer, chunkNumber*instanceSize, amountToRead) |> ignore
      let pages =
          buffer
          |> fun x -> printfn "BYTE CHUNK: %A" x; x
          |> Array.chunkBySize instanceSize
          |> Array.chunkBySize instancesPerPage
          |> Array.chunkBySize instancesPerPage
          |> Array.mapi (fun chunkNumber instances ->
                           instances
                           |> Array.mapi (fun pageNumber page ->
                             page
                             |> Array.mapi
                               (fun slotNumber stream ->
                                    try
                                        deserialize schema entityName stream
                                        |> indexBuilder (chunkNumber*instanceSize+(slotNumber * instanceSize)) chunkNumber pageNumber slotNumber
                                        |> Some
                                    with e -> printfn "ERROR: %A" e.Message; None)
                              |> Array.choose id))
      pages, amountAlreadyRead + amountToRead

  type Pagination =
      { pages: Page array; tree: nativeint; amountAlreadyRead: int }
  
  let buildPagination (reader: System.IO.FileStream) schema entityName (relationToIndex: Map<string, Entity.FieldMetadata>) (indexBuilder: IndexBuilder) initialPageNumber amountAlreadyRead =
      let instanceSize = Schema.relationSize relationToIndex
      let instancesPerPage: int32 = 100_000 / instanceSize
      // use reader = System.IO.File.OpenRead $"/tmp/perplexdb/{entityName}.ndf"
      reader.Seek (0, System.IO.SeekOrigin.Begin) |> ignore
      // let maxPagesAtOnce = 64
      let pages, amountAlreadyRead = buildPages instancesPerPage reader instanceSize amountAlreadyRead 0 indexBuilder schema entityName relationToIndex
      printfn "PAGES: %A" pages
      let tree = Array.fold (fun tree chunk ->
                             Array.fold (fun tree page ->
                               Array.fold (fun tree { key = key
                                                      chunkNumber = chunkNumber
                                                      pageNumber = pageNumber
                                                      slotNumber = slotNumber} ->
                                             Tree.insert(tree, key, chunkNumber, pageNumber, slotNumber)) tree page) tree chunk) IntPtr.Zero pages
      { pages = pages; tree = tree; amountAlreadyRead = amountAlreadyRead }

  [<Struct>]
  type CRecord =
      { chunkNumber: int
        pageNumber: int
        slotNumber: int }
      
  exception ViolationOfConstraint of string

  let update stream (schema: Schema.t) relationName (attributeToUpdate: string) (projection: (Map<string, Value.t>*OffsetNumber option) array) (valueReplace: int32) (constraint: (Expression.Operators*Expression.t list) option) =
      projection
      |> Array.map (fun (x, (offset: OffsetNumber option)) ->
                    Map.change
                        attributeToUpdate
                        (fun v ->
                           match v, constraint with
                           | Some (Value.VInteger32 _), Some (operator, [Expression.LocalizedIdentifier(relation, attribute)])
                               when relation = relationName && operator.GetFunction x.[attribute] valueReplace ->
                               Some (Value.VInteger32 valueReplace)
                           | Some (Value.VInteger32 _), Some (operator, [Expression.LocalizedIdentifier(relation, attribute)])
                               when relation = relationName && not <| operator.GetFunction x.[attribute] valueReplace ->
                               raise <| ViolationOfConstraint (sprintf "Violation of constraint: '%A' is not '%s' to '%A'" (x.[attribute].RawToString()) (constraint.ToString()) valueReplace)
                           | Some (Value.VInteger32 _), None ->
                               Some (Value.VInteger32 valueReplace)
                           | otherwise, _ -> failwithf "Updating '%A' is currently not supported." otherwise)
                        x
                    |> fun x -> (x, offset))
      |> Array.map (fun (x, offset) -> Write.Disk.writeFact stream schema relationName offset x)
      

  let search stream (schema: Schema.t) (entityName: string) (projectionParam: Expression.ProjectionParameter) (refinement: Expression.Operators option) (indexBuilder: IndexBuilder) =
      match Map.tryFind entityName schema with
      | Some (Entity.Relation (relationAttributes, _physicalCount)) ->
          
          let initialPageNumber = 0
          let mutable amountAlreadyRead = 0
          
          // printfn "Leaves: %A" lastReadChunk.pages
          // Tree.print_leaves lastReadChunk.tree
          

          match projectionParam, refinement with
          | Expression.ProjectionParameter.Restrict attributes, None ->
              None
          | Expression.ProjectionParameter.Restrict _, Some (Expression.Operators.Equal (attr, key)) ->
              let lastReadChunk = buildPagination stream schema entityName relationAttributes indexBuilder initialPageNumber amountAlreadyRead
              amountAlreadyRead <- lastReadChunk.amountAlreadyRead
              let value = Tree.find_and_get_value (lastReadChunk.tree, key, false)
              let crecord =
                  Marshal.PtrToStructure<CRecord> value
              printfn "%A" <| crecord              
              printfn "%A" <| lastReadChunk.pages.[crecord.chunkNumber].[crecord.pageNumber].[crecord.slotNumber]
              None
          | Expression.ProjectionParameter.Sum attr, Some (Expression.Operators.Equal (_, key)) ->
              let lastReadChunk = buildPagination stream schema entityName relationAttributes indexBuilder initialPageNumber amountAlreadyRead
              amountAlreadyRead <- lastReadChunk.amountAlreadyRead
              lastReadChunk.pages
              |> Array.concat
              |> Array.concat
              |> Array.choose (function
                                | {entity = entity; key = key_val; offset = offset} when key_val = key ->
                                    let value = entity.[attr]
                                    Some (value, offset)
                                | _ -> None)
              |> Array.sumBy (fun (Value.VInteger32 x, _) -> x)
              |> fun x -> ([|(Map.empty |> Map.add "SUM" (Value.VInteger32 x), None)|])
              |> Some
          | Expression.ProjectionParameter.All, Some (Expression.Operators.Equal (_, key)) ->
              let lastReadChunk = buildPagination stream schema entityName relationAttributes indexBuilder initialPageNumber amountAlreadyRead
              amountAlreadyRead <- lastReadChunk.amountAlreadyRead
              lastReadChunk.pages
              |> Array.concat
              |> Array.concat
              |> Array.choose (function
                                | {entity = entity; key = key_val; offset = offset} when key_val = key ->
                                    //Some (entity |> Map.map (fun _ v -> v.Serialize()), Some offset)
                                    Some (entity, Some offset)
                                | _ -> None)
              |> Some
              
          | Expression.ProjectionParameter.Taking(limit, attributes), Some (Expression.Operators.Equal (_, key)) ->
              let lastReadChunk = buildPagination stream schema entityName relationAttributes indexBuilder initialPageNumber amountAlreadyRead
              amountAlreadyRead <- lastReadChunk.amountAlreadyRead
              lastReadChunk.pages
              |> Array.concat
              |> Array.concat
              |> Array.choose (function
                                | {entity = entity; key = key_val; offset = offset} when key_val = key ->
                                    Some (Map.filter (fun k _ -> List.contains k attributes) entity, Some offset)
                                | _ -> None)
              |> Array.take limit
              |> Some
          | otherwise -> failwithf "NOT EXPECTING: %A" otherwise
          
      | None -> failwithf "Entity '%s' could not be located in the working schema." entityName

end
