using ALAN13featurepack.GameWorld;
using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Linq;

public class LevelController : Node2D, ILevel
{
	[Export]
	int MapControlIncrement = 8;

	[Export]
	double RandomAnimationChance = 0.3;

	[Export]
	public bool DebugEnabled { get; set; }

	[Export]
	public Vector2 CurrentZoomInRate = new Vector2(0.75f, 0.75f);

	[Export]
	public Vector2 MinZoomInRate = new Vector2(0.5f, 0.5f);

	[Export]
	public Vector2 MaxZoomInRate = new Vector2(0.9f, 0.9f);

	public TileGridControl TileGridControl => tileGrid;

	public string AssetName { get; internal set; }

	TileGridControl tileGrid;

	public override void _Ready()
	{
		var tilemaps = this.GetChildren<TileMap>().ToList();

		var ground = tilemaps[0];

		var shadows = tilemaps[1];

		var objects = tilemaps[2];

		tileGrid = new TileGridControl(ground, shadows, objects);

		tileGrid.Scale(CurrentZoomInRate);

	}


}
