// We are going to ask TEM everything
module TEMInterface
open ProgramCore
open FSharp.Stats
open Coords

type TEMInterface (intName : string) = class
    do printfn "Creating TEMInterface: %s" intName
    let mutable name = intName
    member x.Name = name
    member x.getXYCoords() : Coords = 
       DeviceCoords (vector[|1.22; 13.7|])
end