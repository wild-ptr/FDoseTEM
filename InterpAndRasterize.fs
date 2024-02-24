module InterpAndRasterize
open System.Drawing
open WorkerThreadMessages
open ProgramCore
open Coords

// Lets begin the rasterization, shall we?
// Now, since user provided the basis, we know what was the point
// of top left corner, and bottom left corner.
// so... Transforming those two gives us left edge coords, and shows us (in coordinate space)
// position of left top, aka... height in coordinate units.
// Same with bottom right corner, width in coordinate units.

// ... But i will write that when i see the interface. Perhaps we will have to think of
// other way, compatible with how can i capture image so i have heatmap on image later.
// right now lets just forget "convertToPixelCoords" function, ill write it later.
let convertToPixelCoords (coords : Coords) (imageSize : Coords) =
    match (coords, imageSize) with
        | (TransformedCoords coords, PixelCoords(x, y)) -> 
            PixelCoords(coords[0] |> int, coords[1] |> int);
        | _ -> failwith "Wrong coordinate spaces passed."
    // do some logic and transform this shit again, this time to PixelCoords variant.
    // Just a shim for now.

type Circle = 
    {
        Coords : Coords;
        Center : Coords;
    }

// What do i want as result here? perhaps a full description of figure to draw?
// How would i even approach drawing this? I would definitely rasterize those two circles.
// But, i have to be careful not overwrite, as im going to sum, no?
// No, at first i can just have a bitmask! And flip all the bits to one!
// So this is not as crazy fast, since there is need for intermediate mask, or vector of 
// coordinates to flip, but... much easier to implement!
// So ill go with vector of coordinates to flip.

// I guess we can go with 2 circles, and a quad defined by 4 points to rasterize.
// Remember that System.Drawing has functions to directly rasterize that shit to image.
// But i dunno if will be any useful there.
type BeamData =
    {
        CircleOne : Circle
        CircleTwo : Circle
    }


// stop. do it tomorrow u can chill for now
let rec drawPoints x y =
    let points = ResizeArray<_>()

    // Add points considering symmetry in all octants
    points.Add(centerX + x, centerY + y)
    points.Add(centerX + y, centerY + x)
    points.Add(centerX - y, centerY + x)
    points.Add(centerX - x, centerY + y)
    points.Add(centerX - x, centerY - y)
    points.Add(centerX - y, centerY - x)
    points.Add(centerX + y, centerY - x)
    points.Add(centerX + x, centerY - y)

    if x > y then
        y <- y + 1
        if err < 0 then
            err <- err + (2 * y + 1)
        else
            x <- x - 1
            err <- err + (2 * (y - x) + 1)
        drawPoints x y


let getBeamRasterMask (beam : BeamData) : list<Coords> =  
// Many, many coords - just a mask of what coords need to be summed into for irradiance.
    [PixelCoords (2, 5); PixelCoords (3, 2)];

let getCircleDataFromState (info : TEMStateInfo) : Circle = 
    let beamPos = info.Position + info.Offset
    let beamSize = // read weird things from lookup tables.
    0

let rasterizeOnHeatmap(last : TEMStateInfo) (current : TEMStateInfo) (basis : Basis) (bitmap : byref<Bitmap>) =
    0
    

