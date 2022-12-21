using ALAN13featurepack.GameWorld;
using ALAN13featurepack.GameWorld.CharacterStates;
using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using static Godot.Tween;

public class GameCharacter : KinematicBody2D
{
	public event EventHandler<EnteringCellEventArgs> EnteringTileCell;

	protected TileGridControl tileWorld;

	public TileCell CurrentCell;

	public StateController StateController { get; set; }

	public float MoveDuration { get; internal set; }
	public string CharacterName { get; internal set; }

	public Vector2 Direction = new Vector2(0, -1);

	public int SessionEnergyUsed { get; set; }

	public WorldOrientation Orientation = WorldOrientation.SouthEast;

	public TweenController TweenController;

	public AnimatedSprite AnimatedSprite;

	public TileCell TargetCell;

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

	public virtual void UpdateTargetCell()
	{
		TargetCell = tileWorld.GetTargetCell(Position, Direction);
	}

	public virtual void TweenProperty(string property, object initialValue, object finalValue, float duration = -1, EaseType easeType = EaseType.InOut)
	{
		TweenController.InterpolateProperty(this, property, initialValue, finalValue, duration, easeType);
	}
}
