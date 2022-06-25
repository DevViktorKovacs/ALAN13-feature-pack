using ALAN13featurepack.GameWorld;
using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack
{
	/// <summary>
	/// This works as an in memory read-only database
	/// </summary>
    public class StaticData
    {
		public static Dictionary<WorldOrientation, Vector2> OrientationData = new Dictionary<WorldOrientation, Vector2>
		{

			{
				WorldOrientation.NorthEast,

				new Vector2(0,-1)
			},

			{
				WorldOrientation.SouthEast,

				new Vector2(1,0)
			},

			{
				WorldOrientation.SouthWest,

				new Vector2(0,1)
			},

			{
				WorldOrientation.NorthWest,

				new Vector2(-1,0)
			},

		};

		public static Dictionary<CommandKey, string> InputData = new Dictionary<CommandKey, string>
		{
			{CommandKey.TurnLeft , "ui_left" },
			{CommandKey.TurnRight , "ui_right" },
			{CommandKey.MoveForward , "ui_up" },
			{CommandKey.Grab , "ui_down"},
			{CommandKey.ZoomIn , "ui_zoom_in" },
			{CommandKey.ZoomOut , "ui_zoom_out" },
			{CommandKey.PanLeft , "ui_map_left"},
			{CommandKey.PanRight , "ui_map_right" },
			{CommandKey.PanUp , "ui_map_up" },
			{CommandKey.PanDown , "ui_map_down" },
			{CommandKey.Accept , "ui_accept" },
			{CommandKey.Click , "click" },
			{CommandKey.MiddleClick , "middleClick" },
			{CommandKey.RightClick , "rightClick" },
			{CommandKey.None , "" },
			{CommandKey.PageUp , "ui_page_up" },
			{CommandKey.PageDown , "ui_page_down" },
			{CommandKey.SwitchCommandMode, "ui_focus_prev" }
		};

		public static Dictionary<string, Occupant> TileTypes = new Dictionary<string, Occupant>
		{
			{ "N/A", Occupant.Walkable },
			{ "tiles.png 0", Occupant.Walkable },
			{ "tiles.png 1", Occupant.Walkable },
			{ "tiles.png 2", Occupant.Walkable },
			{ "tiles.png 3", Occupant.Obstacle },
			{ "tiles.png 4", Occupant.Obstacle },
		};

		public static Dictionary<string, GroundType> GroundTypes = new Dictionary<string, GroundType>
		{
			{"tiles.png 0", GroundType.Asphalt },
			{"tiles.png 1", GroundType.Sand },
			{"tiles.png 2", GroundType.Grass },
			{"tiles.png 3", GroundType.Grass },
			{"tiles.png 4", GroundType.Grass },
		};

		public static Dictionary<string, StructureData> StructureDataSet = new Dictionary<string, StructureData>
		{

            {
                "buildings.png 0",
                new StructureData
                    {
                        StructureType = StructureType.Wall,
                        Scene = "res://GameWorld/Structures/HouseA.tscn",
                    }
            },

			{
				"buildings.png 1",
				new StructureData
					{
						StructureType = StructureType.Wall,
						Scene = "res://GameWorld/Structures/HouseB.tscn",
					}
			},

			{
				"buildings.png 2",
				new StructureData
					{
						StructureType = StructureType.Wall,
						Scene = "res://GameWorld/Structures/HouseC.tscn",
					}
			},

		};
	}
}

public struct StructureData
{
	public StructureType StructureType;

	public string Scene;

	public Dictionary<WorldOrientation, Vector2> DirectionalOffset;

	public WorldOrientation Orientation;
}
