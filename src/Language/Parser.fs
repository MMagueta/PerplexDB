// Implementation file for parser generated by fsyacc
module Parser

#nowarn "64" // turn off warnings that type variables used in production annotations are instantiated to concrete type

open FSharp.Text.Lexing
open FSharp.Text.Parsing.ParseHelpers

open Language

// This type is the type of tokens accepted by the parser
type token =
    | RIGHT_LIM
    | LEFT_LIM
    | EOF
    | INSERT
    | RELATION
    | CREATE
    | TYPE' of (string)
    | LITERAL_STRING of (string)
    | LITERAL_INTEGER of (int)
    | IDENTIFIER of (string)
// This type is used to give symbolic names to token indexes, useful for error messages
type tokenId =
    | TOKEN_RIGHT_LIM
    | TOKEN_LEFT_LIM
    | TOKEN_EOF
    | TOKEN_INSERT
    | TOKEN_RELATION
    | TOKEN_CREATE
    | TOKEN_TYPE'
    | TOKEN_LITERAL_STRING
    | TOKEN_LITERAL_INTEGER
    | TOKEN_IDENTIFIER
    | TOKEN_end_of_input
    | TOKEN_error
// This type is used to give symbolic names to token indexes, useful for error messages
type nonTerminalId =
    | NONTERM__startstart
    | NONTERM_start
    | NONTERM_File
    | NONTERM_Expression
    | NONTERM_ListAttributes
    | NONTERM_Rev_Attributes
    | NONTERM_ListValues
    | NONTERM_Rev_Values
    | NONTERM_end

// This function maps tokens to integer indexes
let tagOfToken (t: token) =
    match t with
    | RIGHT_LIM -> 0
    | LEFT_LIM -> 1
    | EOF -> 2
    | INSERT -> 3
    | RELATION -> 4
    | CREATE -> 5
    | TYPE' _ -> 6
    | LITERAL_STRING _ -> 7
    | LITERAL_INTEGER _ -> 8
    | IDENTIFIER _ -> 9

// This function maps integer indexes to symbolic token ids
let tokenTagToTokenId (tokenIdx: int) =
    match tokenIdx with
    | 0 -> TOKEN_RIGHT_LIM
    | 1 -> TOKEN_LEFT_LIM
    | 2 -> TOKEN_EOF
    | 3 -> TOKEN_INSERT
    | 4 -> TOKEN_RELATION
    | 5 -> TOKEN_CREATE
    | 6 -> TOKEN_TYPE'
    | 7 -> TOKEN_LITERAL_STRING
    | 8 -> TOKEN_LITERAL_INTEGER
    | 9 -> TOKEN_IDENTIFIER
    | 12 -> TOKEN_end_of_input
    | 10 -> TOKEN_error
    | _ -> failwith "tokenTagToTokenId: bad token"

/// This function maps production indexes returned in syntax errors to strings representing the non terminal that would be produced by that production
let prodIdxToNonTerminal (prodIdx: int) =
    match prodIdx with
    | 0 -> NONTERM__startstart
    | 1 -> NONTERM_start
    | 2 -> NONTERM_start
    | 3 -> NONTERM_File
    | 4 -> NONTERM_Expression
    | 5 -> NONTERM_Expression
    | 6 -> NONTERM_ListAttributes
    | 7 -> NONTERM_ListAttributes
    | 8 -> NONTERM_Rev_Attributes
    | 9 -> NONTERM_Rev_Attributes
    | 10 -> NONTERM_Rev_Attributes
    | 11 -> NONTERM_Rev_Attributes
    | 12 -> NONTERM_ListValues
    | 13 -> NONTERM_ListValues
    | 14 -> NONTERM_Rev_Values
    | 15 -> NONTERM_Rev_Values
    | 16 -> NONTERM_Rev_Values
    | 17 -> NONTERM_Rev_Values
    | 18 -> NONTERM_end
    | _ -> failwith "prodIdxToNonTerminal: bad production index"

let _fsyacc_endOfInputTag = 12
let _fsyacc_tagOfErrorTerminal = 10

// This function gets the name of a token as a string
let token_to_string (t: token) =
    match t with
    | RIGHT_LIM -> "RIGHT_LIM"
    | LEFT_LIM -> "LEFT_LIM"
    | EOF -> "EOF"
    | INSERT -> "INSERT"
    | RELATION -> "RELATION"
    | CREATE -> "CREATE"
    | TYPE' _ -> "TYPE'"
    | LITERAL_STRING _ -> "LITERAL_STRING"
    | LITERAL_INTEGER _ -> "LITERAL_INTEGER"
    | IDENTIFIER _ -> "IDENTIFIER"

// This function gets the data carried by a token as an object
let _fsyacc_dataOfToken (t: token) =
    match t with
    | RIGHT_LIM -> (null: System.Object)
    | LEFT_LIM -> (null: System.Object)
    | EOF -> (null: System.Object)
    | INSERT -> (null: System.Object)
    | RELATION -> (null: System.Object)
    | CREATE -> (null: System.Object)
    | TYPE' _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x
    | LITERAL_STRING _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x
    | LITERAL_INTEGER _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x
    | IDENTIFIER _fsyacc_x -> Microsoft.FSharp.Core.Operators.box _fsyacc_x

let _fsyacc_gotos =
    [| 0us
       65535us
       1us
       65535us
       0us
       1us
       1us
       65535us
       0us
       2us
       1us
       65535us
       0us
       5us
       1us
       65535us
       9us
       10us
       1us
       65535us
       9us
       17us
       1us
       65535us
       14us
       15us
       1us
       65535us
       14us
       28us
       2us
       65535us
       0us
       4us
       2us
       3us |]

let _fsyacc_sparseGotoTableRowOffsets =
    [| 0us; 1us; 3us; 5us; 7us; 9us; 11us; 13us; 15us |]

let _fsyacc_stateToProdIdxsTableElements =
    [| 1us
       0us
       1us
       0us
       1us
       1us
       1us
       1us
       1us
       2us
       1us
       3us
       1us
       4us
       1us
       4us
       1us
       4us
       1us
       4us
       1us
       4us
       1us
       4us
       1us
       5us
       1us
       5us
       1us
       5us
       1us
       5us
       1us
       5us
       3us
       7us
       10us
       11us
       2us
       8us
       9us
       2us
       8us
       9us
       1us
       8us
       1us
       8us
       1us
       8us
       2us
       10us
       11us
       2us
       10us
       11us
       1us
       11us
       1us
       11us
       1us
       11us
       3us
       13us
       16us
       17us
       2us
       14us
       15us
       2us
       14us
       15us
       1us
       14us
       1us
       14us
       1us
       14us
       1us
       14us
       1us
       15us
       2us
       16us
       17us
       2us
       16us
       17us
       1us
       16us
       1us
       17us
       1us
       17us
       1us
       17us
       1us
       17us
       1us
       18us |]

let _fsyacc_stateToProdIdxsTableRowOffsets =
    [| 0us
       2us
       4us
       6us
       8us
       10us
       12us
       14us
       16us
       18us
       20us
       22us
       24us
       26us
       28us
       30us
       32us
       34us
       38us
       41us
       44us
       46us
       48us
       50us
       53us
       56us
       58us
       60us
       62us
       66us
       69us
       72us
       74us
       76us
       78us
       80us
       82us
       85us
       88us
       90us
       92us
       94us
       96us
       98us |]

let _fsyacc_action_rows = 44

let _fsyacc_actionTableElements =
    [| 3us
       32768us
       2us
       43us
       3us
       12us
       5us
       6us
       0us
       49152us
       1us
       32768us
       2us
       43us
       0us
       16385us
       0us
       16386us
       0us
       16387us
       1us
       32768us
       4us
       7us
       1us
       32768us
       9us
       8us
       1us
       32768us
       1us
       9us
       1us
       16390us
       9us
       18us
       1us
       32768us
       0us
       11us
       0us
       16388us
       1us
       32768us
       9us
       13us
       1us
       32768us
       1us
       14us
       1us
       16396us
       9us
       29us
       1us
       32768us
       0us
       16us
       0us
       16389us
       1us
       16391us
       9us
       23us
       1us
       32768us
       6us
       19us
       1us
       16393us
       1us
       20us
       1us
       32768us
       8us
       21us
       1us
       32768us
       0us
       22us
       0us
       16392us
       1us
       32768us
       6us
       24us
       1us
       16394us
       1us
       25us
       1us
       32768us
       8us
       26us
       1us
       32768us
       0us
       27us
       0us
       16395us
       1us
       16397us
       9us
       36us
       1us
       32768us
       6us
       30us
       2us
       32768us
       1us
       31us
       8us
       35us
       1us
       32768us
       8us
       32us
       1us
       32768us
       0us
       33us
       1us
       32768us
       7us
       34us
       0us
       16398us
       0us
       16399us
       1us
       32768us
       6us
       37us
       2us
       32768us
       1us
       39us
       8us
       38us
       0us
       16400us
       1us
       32768us
       8us
       40us
       1us
       32768us
       0us
       41us
       1us
       32768us
       7us
       42us
       0us
       16401us
       0us
       16402us |]

let _fsyacc_actionTableRowOffsets =
    [| 0us
       4us
       5us
       7us
       8us
       9us
       10us
       12us
       14us
       16us
       18us
       20us
       21us
       23us
       25us
       27us
       29us
       30us
       32us
       34us
       36us
       38us
       40us
       41us
       43us
       45us
       47us
       49us
       50us
       52us
       54us
       57us
       59us
       61us
       63us
       64us
       65us
       67us
       70us
       71us
       73us
       75us
       77us
       78us |]

let _fsyacc_reductionSymbolCounts =
    [| 1us
       2us
       1us
       1us
       6us
       5us
       0us
       1us
       5us
       2us
       3us
       6us
       0us
       1us
       6us
       3us
       4us
       7us
       1us |]

let _fsyacc_productionToNonTerminalTable =
    [| 0us
       1us
       1us
       2us
       3us
       3us
       4us
       4us
       5us
       5us
       5us
       5us
       6us
       6us
       7us
       7us
       7us
       7us
       8us |]

let _fsyacc_immediateActions =
    [| 65535us
       49152us
       65535us
       16385us
       16386us
       16387us
       65535us
       65535us
       65535us
       65535us
       65535us
       16388us
       65535us
       65535us
       65535us
       65535us
       16389us
       65535us
       65535us
       65535us
       65535us
       65535us
       16392us
       65535us
       65535us
       65535us
       65535us
       16395us
       65535us
       65535us
       65535us
       65535us
       65535us
       65535us
       16398us
       16399us
       65535us
       65535us
       16400us
       65535us
       65535us
       65535us
       16401us
       16402us |]

let _fsyacc_reductions =
    lazy
        [| (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> AST.Expression option in

               Microsoft.FSharp.Core.Operators.box (
                   (raise (FSharp.Text.Parsing.Accept(Microsoft.FSharp.Core.Operators.box _1))): 'gentype__startstart
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_File in
               let _2 = parseState.GetInput(2) :?> 'gentype_end in
               Microsoft.FSharp.Core.Operators.box ((_1): AST.Expression option))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_end in
               Microsoft.FSharp.Core.Operators.box ((None): AST.Expression option))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Expression in
               Microsoft.FSharp.Core.Operators.box ((Some _1): 'gentype_File))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _3 = parseState.GetInput(3) :?> string in
               let _5 = parseState.GetInput(5) :?> 'gentype_ListAttributes in

               Microsoft.FSharp.Core.Operators.box (
                   (AST.Expression.CreateRelation(_3, Map.ofList _5)): 'gentype_Expression
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _2 = parseState.GetInput(2) :?> string in
               let _4 = parseState.GetInput(4) :?> 'gentype_ListValues in

               Microsoft.FSharp.Core.Operators.box (
                   (AST.Expression.Insert(
                       _2,
                       List.map
                           (fun ((identifier: string), type', value) ->
                               { FieldName = identifier
                                 FieldType = type'
                                 FieldValue = value }
                               : AST.InsertFieldInfo)
                           _4
                       |> Array.ofList
                   ))
                   : 'gentype_Expression
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               Microsoft.FSharp.Core.Operators.box (([]): 'gentype_ListAttributes))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Attributes in
               Microsoft.FSharp.Core.Operators.box ((List.rev _1): 'gentype_ListAttributes))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> string in
               let _2 = parseState.GetInput(2) :?> string in
               let _4 = parseState.GetInput(4) :?> int in

               Microsoft.FSharp.Core.Operators.box (
                   (match _2 with
                    | "VARCHAR" -> [ (_1, AST.Types.VariableCharacters _4) ])
                   : 'gentype_Rev_Attributes
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> string in
               let _2 = parseState.GetInput(2) :?> string in

               Microsoft.FSharp.Core.Operators.box (
                   (match _2 with
                    | "INTEGER" -> [ (_1, AST.Types.Integer32) ]
                    | "VARCHAR" -> failwith "VARCHAR is a parametric type, therefore it requires a size.")
                   : 'gentype_Rev_Attributes
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Attributes in
               let _2 = parseState.GetInput(2) :?> string in
               let _3 = parseState.GetInput(3) :?> string in

               Microsoft.FSharp.Core.Operators.box (
                   ((_2,
                     match _3 with
                     | "INTEGER" -> AST.Types.Integer32
                     | "VARCHAR" -> failwith "VARCHAR is a parametric type, therefore it requires a size.")
                    :: _1)
                   : 'gentype_Rev_Attributes
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Attributes in
               let _2 = parseState.GetInput(2) :?> string in
               let _3 = parseState.GetInput(3) :?> string in
               let _5 = parseState.GetInput(5) :?> int in

               Microsoft.FSharp.Core.Operators.box (
                   ((_2,
                     match _3 with
                     | "VARCHAR" -> AST.Types.VariableCharacters _5)
                    :: _1)
                   : 'gentype_Rev_Attributes
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               Microsoft.FSharp.Core.Operators.box (([]): 'gentype_ListValues))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Values in
               Microsoft.FSharp.Core.Operators.box ((List.rev _1): 'gentype_ListValues))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> string in
               let _2 = parseState.GetInput(2) :?> string in
               let _4 = parseState.GetInput(4) :?> int in
               let _6 = parseState.GetInput(6) :?> string in

               Microsoft.FSharp.Core.Operators.box (
                   (match _2 with
                    | "VARCHAR" -> [ (_1, AST.Types.VariableCharacters _4, AST.ELiteral.LVarChar _6) ])
                   : 'gentype_Rev_Values
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> string in
               let _2 = parseState.GetInput(2) :?> string in
               let _3 = parseState.GetInput(3) :?> int in

               Microsoft.FSharp.Core.Operators.box (
                   (match _2 with
                    | "INTEGER" -> [ (_1, AST.Types.Integer32, AST.ELiteral.LInteger _3) ]
                    | "VARCHAR" -> failwith "VARCHAR is a parametric type, therefore it requires a size.")
                   : 'gentype_Rev_Values
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Values in
               let _2 = parseState.GetInput(2) :?> string in
               let _3 = parseState.GetInput(3) :?> string in
               let _4 = parseState.GetInput(4) :?> int in

               Microsoft.FSharp.Core.Operators.box (
                   ((_2,
                     (match _3 with
                      | "INTEGER" -> AST.Types.Integer32
                      | "VARCHAR" -> failwith "VARCHAR is a parametric type, therefore it requires a size."),
                     AST.ELiteral.LInteger _4)
                    :: _1)
                   : 'gentype_Rev_Values
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               let _1 = parseState.GetInput(1) :?> 'gentype_Rev_Values in
               let _2 = parseState.GetInput(2) :?> string in
               let _3 = parseState.GetInput(3) :?> string in
               let _5 = parseState.GetInput(5) :?> int in
               let _7 = parseState.GetInput(7) :?> string in

               Microsoft.FSharp.Core.Operators.box (
                   ((_2,
                     (match _3 with
                      | "VARCHAR" -> AST.Types.VariableCharacters _5),
                     AST.ELiteral.LVarChar _7)
                    :: _1)
                   : 'gentype_Rev_Values
               ))
           (fun (parseState: FSharp.Text.Parsing.IParseState) ->
               Microsoft.FSharp.Core.Operators.box ((None): 'gentype_end)) |]

let tables: FSharp.Text.Parsing.Tables<_> =
    { reductions = _fsyacc_reductions.Value
      endOfInputTag = _fsyacc_endOfInputTag
      tagOfToken = tagOfToken
      dataOfToken = _fsyacc_dataOfToken
      actionTableElements = _fsyacc_actionTableElements
      actionTableRowOffsets = _fsyacc_actionTableRowOffsets
      stateToProdIdxsTableElements = _fsyacc_stateToProdIdxsTableElements
      stateToProdIdxsTableRowOffsets = _fsyacc_stateToProdIdxsTableRowOffsets
      reductionSymbolCounts = _fsyacc_reductionSymbolCounts
      immediateActions = _fsyacc_immediateActions
      gotos = _fsyacc_gotos
      sparseGotoTableRowOffsets = _fsyacc_sparseGotoTableRowOffsets
      tagOfErrorTerminal = _fsyacc_tagOfErrorTerminal
      parseError =
        (fun (ctxt: FSharp.Text.Parsing.ParseErrorContext<_>) ->
            match parse_error_rich with
            | Some f -> f ctxt
            | None -> parse_error ctxt.Message)
      numTerminals = 13
      productionToNonTerminalTable = _fsyacc_productionToNonTerminalTable }

let engine lexer lexbuf startState =
    tables.Interpret(lexer, lexbuf, startState)

let start lexer lexbuf : AST.Expression option = engine lexer lexbuf 0 :?> _
