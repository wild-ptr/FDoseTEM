module WorkerThreadMessages
open ProgramCore 

type TEMStateInfo =
    {
        Position : Coords
        Offset : Coords
        Time : System.TimeSpan
        Width : float
        Dose : float
        Power : float
    }


type ConfigurationInfo = 
    {
        Basis : Basis
        BitmapSize : int * int
    }

type WorkerMessage =
    | Task of state : TEMStateInfo
    | Finish