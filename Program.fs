open System
open ProgramCore
open FSharp.Stats
open Coords
open TEMInterface
open WorkerThread
open WorkerThreadMessages

let private askUserForBasis (inter : TEMInterface) : Basis =
    printfn "Point TEM to lower left corner of sample and press enter"
    let aaa = Console.ReadLine
    let x_coord = inter.getXYCoords()

    printfn "Point TEM to lower right corner of sample and press enter"
    let xxx = Console.ReadLine
    let x_right_coord = inter.getXYCoords()
    
    //let x_vec = Coords.sameCoordsAction x_coord x_right_coord (fun x y -> x - y)
    let x_vec = x_coord - x_right_coord

    let y_vec = DeviceCoords (vector[|Coords.getY(-x_vec); Coords.getX x_vec|])
    {x_vec = x_vec;
     y_vec = y_vec;
     point = x_coord }



[<EntryPoint>]
let main args =
    let interf = TEMInterface "interfejs1"
    let basis = askUserForBasis interf
    let transformPoint p = Basis.transformWithBasis basis p

    let config : ConfigurationInfo = 
        {
            Basis = basis;
            BitmapSize = (1000, 1000) // not used for now
        }

    let heatmapCalcTask = DrawHeatmapTask config

    let heatmapCalcThread = MailboxProcessor.Start(fun (inbox : MailboxProcessor<WorkerMessage>) ->
        heatmapCalcTask.runnerFunc inbox)
    

    // This is how to send shit to thread.
    //heatmapCalcThread.Post()
    heatmapCalcThread.Post Finish
    // We should start packing data here and Posting it to worker.

    // After all work is done we wait for results to be calculated completely.
    let heatmap = heatmapCalcTask.getResult();

    0

