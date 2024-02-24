module ProgramCore
open FSharp.Stats

type Coords =
    | DeviceCoords of vec : vector
    | TransformedCoords of vec : vector
    | PixelCoords of x : int * y : int

type Basis = {
    x_vec : Coords
    y_vec : Coords
    point : Coords
}

let (~-) (coords :Coords) =
    match coords with
    | DeviceCoords vec -> DeviceCoords(-vec)
    | TransformedCoords vec -> TransformedCoords(-vec)
    | PixelCoords (x,y) -> PixelCoords(-x, -y)

module Basis =
    let zeroCreate () = 
        let zero = DeviceCoords (vector [|0.; 0. ; 0. ; 0.|]);
        { 
            x_vec = zero;
            y_vec = zero;
            point = zero;
        }
    // Could be memoized, doesnt have to.
    let getBasisTransformMatrix (basis : Basis) : Matrix<float> = 
        match (basis.x_vec, basis.y_vec) with
        | DeviceCoords x_vec, DeviceCoords y_vec -> 
            matrix [[x_vec[0]; y_vec[0]]
                    [x_vec[1]; y_vec[1]]]
        | _ -> failwith "Must be in device coordinates"
    let transformWithBasis (basis : Basis) (DeviceCoords coords)=
        let pointToTransform = match coords with
                                    | vec -> vec

        let translationVector = match basis.point with
                                | DeviceCoords c -> c
        
        let BasisMatrix = getBasisTransformMatrix basis
        let translationPostBasisChange = BasisMatrix * translationVector
        TransformedCoords ((BasisMatrix * pointToTransform) + translationPostBasisChange)
        
module Coords = 
    let sameCoordsAction (c1: Coords) (c2: Coords) (f : vector -> vector -> vector) : Coords =
        match (c1, c2) with
        | (DeviceCoords v1, DeviceCoords v2) -> DeviceCoords(f v1 v2)
        | (TransformedCoords v1, TransformedCoords v2) -> TransformedCoords(f v1 v2)
        | _ -> failwith "Coords must be in the same space for op to be meaningful."

    let getY (coords : Coords) = 
        match coords with
        | DeviceCoords vec
        | TransformedCoords vec -> vec[1]

    let getX (coords : Coords) =
        match coords with
        | DeviceCoords vec
        | TransformedCoords vec -> vec[0]

    let (-) (c1 : Coords) (c2: Coords) = 
        sameCoordsAction c1 c2 (fun (x : vector) (y : vector) -> x - y)

    let (+) (c1 : Coords) (c2: Coords) = //
        sameCoordsAction c1 c2 (fun x y -> x + y)
