using ALAN13featurepack;
using ALAN13featurepack.GameWorld;
using ALAN13featurepack.GameWorld.CharacterStates;
using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Linq;
using static Godot.Tween;

public class GameCharacter : KinematicBody2D
{
	public event EventHandler<EnteringCellEventArgs> EnteringTileCell;

	public event EventHandler<CommandFinishedEventArgs> CommandFinished;

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

	protected Tween tween;

	public float DefaultAnimationSpeed { get; set; }

	protected float animationSpeed = 1;

	public override void _Ready()
	{
		AnimatedSprite = this.GetChild<AnimatedSprite>();

		DefaultAnimationSpeed = 1;

		tween = this.GetChild<Tween>();

		TweenController = new TweenController(115, 90, tween, MoveDuration);

		StateController = new StateController(this, this.GetChildren<Timer>().ToList());

		TweenController.Randomize = false;

		TweenController.Subject = this;
	}

	public override void _UnhandledKeyInput(InputEventKey @event)
	{
		if (Input.IsActionJustReleased(StaticData.InputData[CommandKey.TurnRight]))
		{
			DebugHelper.PrettyPrintVerbose(CommandKey.TurnRight, ConsoleColor.DarkGray);
			StateController.IssueCommand(CommandKey.TurnRight, this);
			return;
		}

		if (Input.IsActionJustReleased(StaticData.InputData[CommandKey.TurnLeft]))
		{
			DebugHelper.PrettyPrintVerbose(CommandKey.TurnLeft, ConsoleColor.DarkGray);
			StateController.IssueCommand(CommandKey.TurnLeft, this);
			return;
		}

		if (Input.IsActionJustReleased(StaticData.InputData[CommandKey.MoveForward]))
		{
			DebugHelper.PrettyPrintVerbose(CommandKey.MoveForward, ConsoleColor.DarkGray);
			StateController.IssueCommand(CommandKey.MoveForward, this);
			return;
		}

		if (Input.IsActionJustReleased(StaticData.InputData[CommandKey.Grab]))
		{
			DebugHelper.PrettyPrintVerbose(CommandKey.Grab, ConsoleColor.DarkGray);
			StateController.IssueCommand(CommandKey.Grab, this);
			return;
		}
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

	public void OnCommandFinished(CommandFinishedEventArgs e)
	{
		CommandFinished?.Invoke(this, e);
	}
}
