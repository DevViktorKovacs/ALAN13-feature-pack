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

	TileCell activeCell;

	Vector2 startingVector = new Vector2();

	public override void _Ready()
	{
		var tilemaps = this.GetChildren<TileMap>().ToList();

		var ground = tilemaps[0];

		var shadows = tilemaps[1];

		var objects = tilemaps[2];

		tileGrid = new TileGridControl(ground, shadows, objects);

		tileGrid.Scale(CurrentZoomInRate);

		this.InputProcessor().MouseScrollDown += LevelController_MouseScrollDown;

		this.InputProcessor().MouseScrollUp += LevelController_MouseScrollUp;

		this.InputProcessor().MouseLeftUp += LevelController_MouseLeftUp;

		this.InputProcessor().MouseLeftDown += LevelController_MouseLeftDown;

		this.InputProcessor().MouseDragged += LevelController_MouseDragged; 

		this.InputProcessor().MousePanned += LevelController_MousePanned;

		this.InputProcessor().KeyPressed += LevelController_KeyPressed;

	}

	private void LevelController_MouseDragged(object sender, InputEventArgs e)
	{
		if (activeCell == null) return;
		if (activeCell.Structure == null) return;

		activeCell.Structure.StructureSprites.Offset = startingVector - e.Offset;

		if (activeCell.Structure.Shadow != null)
		{
			activeCell.Structure.Shadow.Offset = startingVector - e.Offset;
		}

		if (activeCell.Structure.Content != null)
		{
			var sprites = activeCell.Structure.Content.Position = startingVector - e.Offset;
		}

		DebugHelper.PrettyPrintVerbose($"New offset: {activeCell.Structure.StructureSprites.Offset}");
	}

	private void LevelController_MouseLeftDown(object sender, InputEventArgs e)
	{
		var cell = tileGrid.GetCellAtMousePosition();

		DebugHelper.PrettyPrintVerbose(cell, ConsoleColor.DarkGray);

		var structure = cell.Structure;

		if (structure == null)
		{
			activeCell = null;

			return;
		}

		DebugHelper.PrettyPrint($"{structure} grabbed", ConsoleColor.Green);

		var sprites = structure.StructureSprites;

		activeCell = cell;

		startingVector = sprites.Offset;
	}

	private void LevelController_KeyPressed(object sender, InputEventArgs e)
	{
		//throw new NotImplementedException();
	}

	private void LevelController_MousePanned(object sender, InputEventArgs e)
	{
		tileGrid.ShiftMap((-1) * e.Offset);
	}

	private void LevelController_MouseLeftUp(object sender, InputEventArgs e)
	{
		tileGrid.SelectCell();
	}

	private void LevelController_MouseScrollUp(object sender, InputEventArgs e)
	{
		if (CurrentZoomInRate > MinZoomInRate)
		{
			CurrentZoomInRate.x -= 0.01f;

			CurrentZoomInRate.y -= 0.01f;
		}

		tileGrid.Scale(CurrentZoomInRate);
	}

	private void LevelController_MouseScrollDown(object sender, InputEventArgs e)
	{
		if (CurrentZoomInRate < MaxZoomInRate)
		{
			CurrentZoomInRate.x += 0.01f;

			CurrentZoomInRate.y += 0.01f;
		}

		tileGrid.Scale(CurrentZoomInRate); ;
	}
}
