module Main

[<RequireQualifiedAccess>] // avoid conflict with HTMLAttr.Start
type Msg = Start | Stop | NoOp

type Status = Off | On

type Model =
    { Status: Status
      Transitions: bool }

let init () =
    { Status = Off
      Transitions = false }, []

let update msg model =
    match msg with 
    | Msg.Start ->
        {model with Status = On}, []
    | Msg.Stop ->
        {model with Status = Off}, []
    | Msg.NoOp ->
        model, []
    |> fun (m, fx) ->
        // always false after hot reload
        {m with Transitions = m.Status = On}, fx

open Fable.React
open Fable.React.Props

// something to comment/uncomment to trigger hot reload
let hotReloadTrigger () = ()

let view model dispatch =
    div [Style [Padding "0 24px"; FontFamily "sans-serif"; FontSize "14pt"]] [
        h4 [] [str "Repro"]
        ol [] [
            li [] [str "Click Start, observe:"; br []; b [] [code [] [str "Transitions = true"]]]
            li [] [str "Click Update, observe nothing changes"]
            li [] [str "Comment or uncomment "; code [] [str "hotReloadTrigger"]; str " and save"]
            li [] [str "Click Update, observe: "; br []; b [] [code [] [str "Transitions = false"]]]
        ]
        br []
        match model.Status with
        | Off ->
            button [Style [FontSize "14pt"]; OnClick (fun _ -> dispatch Msg.Start)] [str "Start"]
        | On ->
            button [Style [FontSize "14pt"]; OnClick (fun _ -> dispatch Msg.Stop)] [str "Stop"]
        button [Style [FontSize "14pt"; MarginLeft "12px"]
                OnClick (fun _ -> dispatch Msg.NoOp)]
               [str "Update"]
        br []
        br []
        pre [] [str (sprintf "%A" model)]
    ]

open Elmish
open Elmish.React
open Elmish.HMR // shadows Program.<fn>

let initArg = ()

Program.mkProgram init update view
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.runWith initArg
