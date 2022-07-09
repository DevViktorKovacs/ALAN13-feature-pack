using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.Interfaces
{
    public enum GroundType
    {
        Unknown = 0,
        Grass = 1,
        Sand = 2,
        Asphalt = 3,
        Snow = 4
    }

    public enum Occupant
    {
        Walkable = 0,
        Player = 1,
        Obstacle = 2,
        Collectable = 3,
        Occupied = 4,
        Hole = 5,
        Wall = 6,
        GreenCrystal = 7,
        RedCrystal = 8,
        ConveyorBelt = 9,
        Void = 10,
        Virtual = 11,
        Portal = 12,
        DefaultTile = 13,
        Closed = 14,
    }

    public enum WorldOrientation
    {
        SouthEast = 0,
        SouthWest = 1,
        NorthWest = 2,
        NorthEast = 3,
    }

    public enum StructureType
    {
        Hole = 0,
        Wall = 1,
        Doodad = 10,

    }

    public enum CommandKey
    {
        TurnRight = 0,
        TurnLeft = 1,
        MoveForward = 2,
        Grab = 3,
        ZoomIn = 4,
        ZoomOut = 5,
        PanLeft = 6,
        PanRight = 7,
        PanUp = 8,
        PanDown = 9,
        Accept = 10,
        None = 11,
        Idle = 12,
        Click = 13,
        MiddleClick = 14,
        PageUp = 15,
        PageDown = 16,
        SwitchCommandMode = 17,
        Walk = 18,
        RightClick = 19,
        Skip = 20,
    }

    public enum AssetKeys
    {
        Empty = 0,
        FirstLevel = 1,
    }

    public enum StateEnum
    {
        Idle = 18,
        Turn = 2,
        Move = 1,
        Dead = 8,
        SmartMove = 19,
    }

    public enum AnimationKeys
    {
        Default = 0,
        TurnLeft = 8,
        TurnRight = 9,
        NorthEast = 10,
        NorthWest = 11,
        SouthEast = 12,
        SouthWest = 13,
        Idle = 16,
        StartWalk = 17,
        StopWalk = 18,
        Walk = 19,
        LeftClick = 26,
        RightHold = 27,
        Scroll = 28,
        LeftHold = 29,
    }
}
