using ALAN13featurepack.GameWorld;
using ALAN13featurepack.GameWorld.CharacterStates;
using Godot;
using System;

public class GameCharacter : KinematicBody2D
{
	public event EventHandler<EnteringCellEventArgs> EnteringTileCell;

	protected TileGridControl tileWorld;

	public TileCell CurrentCell;
	public override void _Ready()
	{
		
	}

	public TileCell GetCurrentCell()
	{
		var result = tileWorld.GetTargetCell(Position, new Vector2(0, 0));

		if (result != CurrentCell)
		{
			var e = new EnteringCellEventArgs() { OldCell = CurrentCell, NewCell = result };

			CurrentCell = result;

			EnteringTileCell?.Invoke(this, e);
		}

		return result;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
