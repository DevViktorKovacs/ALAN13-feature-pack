using ALAN13featurepack;
using ALAN13featurepack.GameWorld;
using ALAN13featurepack.GameWorld.CharacterStates;
using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Linq;

public class GameCharacter : KinematicBody2D
{
	public event EventHandler<EnteringCellEventArgs> EnteringTileCell;

	public event EventHandler<CommandFinishedEventArgs> CommandFinished;

	public TileCell CurrentCell { get; set; }

	public StateController StateController { get; set; }

	public TweenController TweenController { get; set; }

	public AnimatedSprite AnimatedSprite { get; set; }

	public TileCell TargetCell { get; set; }

	public TileGridControl TileWorld
	{
		get => tileWorld;
		set => tileWorld = value;
	}

	public int SessionEnergyUsed { get; set; }

	public string CharacterName { get; internal set; }

	public float DefaultAnimationSpeed { get; set; }

	public float MoveDuration => moveDuration;

	public Vector2 Direction = new Vector2(1, 0);

	public WorldOrientation Orientation = WorldOrientation.SouthEast;

	protected Tween tween;

	protected TileGridControl tileWorld;

	protected AnimationFactory animationFactory = new AnimationFactory();

	protected float moveDuration = 1f;

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

	public virtual void TweenProperty(string property, object initialValue, object finalValue, float duration = -1, Tween.EaseType easeType = Tween.EaseType.InOut)
	{
		TweenController.InterpolateProperty(this, property, initialValue, finalValue, duration, easeType);
	}

	public void OnCommandFinished(CommandFinishedEventArgs e)
	{
		CommandFinished?.Invoke(this, e);
	}

	public virtual long PlayOrientationSpecificAnimation(string animationName, WorldOrientation orientation, bool backwards = false)
	{
		DebugHelper.PrettyPrintVerbose($"{CharacterName}: Playing animation {animationName}{orientation}", ConsoleColor.Green);

		long result = 0;

		string animationFullName;

		ResetAnimation();

		animationFullName = $"{animationName}{StaticData.AnimationData[StaticData.GetAnimationKeysFromOrientation(orientation)]}";

		AnimatedSprite.Play(animationFullName, backwards);

		if (!animationFullName.Contains(StaticData.AnimationData[AnimationKeys.Idle]))
		{
			result = animationFactory.PushRecord(AnimatedSprite, animationFullName);
		}

		return result;
	}

	public void ResetAnimation()
	{
		AnimatedSprite.Frame = 0;
	}
	private void _on_AnimatedSprite_animation_finished()
	{
		var currentAnimationName = AnimatedSprite.Animation;

		var record = animationFactory.PopRecord(currentAnimationName);

		if (record == default) return;

		var e = new AnimationFinishedEventArgs()
		{
			FrameCount = record.FrameCount,

			Frame = AnimatedSprite.Frame,

			AnimationName = record.AnimationName,

			AnimiationID = record.ID
		};

		DebugHelper.PrettyPrintVerbose("Animation finished:", ConsoleColor.Green);

		DebugHelper.PrettyPrintVerbose($"{currentAnimationName} (frame:{e.Frame} id: {e.AnimiationID})", ConsoleColor.Green);

		StateController.AnimationFinishedAction?.Invoke(this, e);
	}


	private void _on_Tween_tween_all_completed()
	{
		StateController.TweenFinishedAction?.Invoke(this, new EventArgs());
	}
}



