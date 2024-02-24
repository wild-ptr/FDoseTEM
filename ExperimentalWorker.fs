module ExperimentalWorker
open System
open ProgramCore
open Coords
open Basis

type MeasurementObservable<'data> () =
  let observers = ResizeArray<IObserver<'data>>()

  member _.report (value: 'data) =
    observers
    |> Seq.iter (fun obs -> obs.OnNext value)

  interface IObservable<'data> with
    member _.Subscribe observer =
      if Seq.contains observer observers then ()
      else observers.Add observer
      // just a dtor
      { new IDisposable with
          member _.Dispose() =
            if observer <> null && observers.Contains observer then
              observers.Remove observer |> ignore }


// This is our async measurement task, we shall run it in the background
// and get new results to processing queue.

// very important is this observable we created, it serves as conduit between the 
// asynchronous reading task (producer) and the observer (working thread).
// This function returns a tuple, hence the syntax (of async task and the iobservable)

let createMeasurementSystem config : Async<unit> * IObservable<int> =
  // Do whatever setup you need to with the config (user input and/or basis matrix)
  let _ = config

    // get Basis Matrix int
  let observable = MeasurementObservable<int>()

  // Here we are just putting together a fake background task that will report random numbers at regular intervals
  // This would get replaced with your actual measurement tasks, wrapped in an Async<'T>
  let rng = Random()
  let task = async {
    // Here we would do any setup needed in the background - perhaps setting up your com port or whatnot.

    // And this loop is just representing whatever sequential processing the instrument needs to do.
    for _ in 1 .. 20 do
      do! Async.Sleep 500
      observable.report (rng.Next())
  }

  // We return both the task and the observable that the task will be updating
  task, observable

let config = () // get your config from your user
let task, obs = createMeasurementSystem config

obs
|> Observable.scan (fun data datum -> datum :: data) []
|> Observable.subscribe (printfn "Collected so far: %A")

Async.RunSynchronously task
