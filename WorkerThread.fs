module WorkerThread
open TEMInterface
open ProgramCore
open Coords
open System
open System.Drawing
open System.Threading
open WorkerThreadMessages
open InterpAndRasterize
// Asynchronously read and passed to worker thread.
// Device coords, raw data from TEM

type DrawHeatmapTask (config : ConfigurationInfo) =
    let cfg : ConfigurationInfo = config
    let mutable lastSample : TEMStateInfo option = None

    //TODO: Make this not hardcoded later xDD
    let BITMAP_WIDTH = 1000 
    let BITMAP_HEIGHT = 1000 
    let mutable heatmap : System.Drawing.Bitmap = new Bitmap(BITMAP_WIDTH, BITMAP_HEIGHT)
    let mutable workFinishedSemaphore : ManualResetEvent = new ManualResetEvent(false)

    let getLast = 
        match lastSample with
        | Some x -> x
        | None -> failwith "Tried to use empty last measurement"

    member x.getResult() = 
        //Wait on the semaphore from finish msg
        workFinishedSemaphore.WaitOne() |> ignore
        // All processing done here
        heatmap

    member x.getBasis () = cfg.Basis
    member private x.transformState (info : TEMStateInfo) = 
        match (info.Position, info.Offset) with
        | (DeviceCoords pos, DeviceCoords off) ->
            { info with 
                Position = Basis.transformWithBasis (x.getBasis()) info.Position;
                Offset = Basis.transformWithBasis (x.getBasis()) info.Offset;
            }
        | (_, _) -> failwith "Worker thread accepts only device-coord messages"

    member private x.processData (info : TEMStateInfo) : unit = 
        if lastSample.IsNone then
            lastSample <- Some (x.transformState info)
        else 
            let stateT = x.transformState info
            let posT = stateT.Position + stateT.Offset
            let PosTlast = getLast.Position + getLast.Offset

            rasterizeOnHeatmap(lastSample stateT &x.heatmap)
            lastSample <- Some info
        
    member x.runnerFunc (mailbox : MailboxProcessor<WorkerMessage>) =
        async {
            let! msg = mailbox.Receive();
            match msg with
            | Task info -> x.processData info
            | Finish -> workFinishedSemaphore.Set() |> ignore;
        }
