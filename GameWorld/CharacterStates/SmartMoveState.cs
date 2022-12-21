using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
	class SmartMoveState : State, ICharacterState
	{
		protected List<TileCell> routeCells;

		protected int nextCheckPointIndex;

		protected int routeCellsIndex;

		protected bool checkPointReached;

		protected int nextPointGridDistance;

		protected bool accelerating;

		protected bool inProgress;

		protected bool interruptionRequested;

		protected TileCell newTarget;

		public SmartMoveState(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler) : base(subject, eventHandler)
		{
			InstanceState = StateEnum.SmartMove;

			TweenFinishedHandler = Parent_TweenFinished;

			EventTimerTimedOutHandler = Parent_TimerTimedOut;

			AnimationFinishedHandler = Parent_AnimationFinished;

			Reset();
		}

		public override void Reset()
		{
			NextState = DefaultNextState;

			routeCells = new List<TileCell>();

			nextCheckPointIndex = 0;

			routeCellsIndex = 0;

			checkPointReached = false;

			nextPointGridDistance = 0;

			accelerating = false;

			inProgress = false;

			interruptionRequested = false;
		}

		protected virtual void Parent_TweenFinished(object sender, EventArgs e)
		{
			if (!Active) return;

			DebugHelper.PrettyPrintVerbose($"RouteCellIndex: {routeCellsIndex}/{routeCells.Count}", ConsoleColor.DarkGray);

			if (interruptionRequested)
			{
				interruptionRequested = false;

				if (CalculatePath(newTarget))
				{
					EvaluateNextMove();

					return;
				}

				FinishState();

				return;
			}

			if (IsTurningRequired())
			{
				EvaluateNextMove();

				return;
			}

			FinishState();
		}

		private bool IsTurningRequired()
		{
			return checkPointReached && routeCellsIndex < routeCells.Count;
		}

		protected virtual void Parent_AnimationFinished(object arg1, AnimationFinishedEventArgs arg2)
		{
			if (interruptionRequested)
			{
				interruptionRequested = false;

				if (CalculatePath(newTarget))
				{
					EvaluateNextMove();

					return;
				}

				FinishState();
			}

			EvaluateNextMove();
		}

		protected virtual void Parent_TimerTimedOut(object arg1, EventArgs arg2)
		{
			return;
		}

		public void SubscribeToEvents()
		{
			Subject.EnteringTileCell += Robot_EnteringTileCell;
		}

		public void UnSubscribeFromEvents()
		{
			Subject.EnteringTileCell -= Robot_EnteringTileCell;
		}

		protected virtual void Robot_EnteringTileCell(object sender, EnteringCellEventArgs e)
		{
			DebugHelper.PrettyPrintVerbose($"{Subject.Name}: entering new cell {e.NewCell.GridPosition}", ConsoleColor.DarkGray);

			routeCellsIndex++;

			if (interruptionRequested)
			{
				Decelerate();

				checkPointReached = true;

				return;
			}

			if (accelerating
				&& routeCells.TryToGetValue(nextCheckPointIndex, out TileCell result)
				&& e.NewCell != result
				&& routeCells.TryToGetValue(nextCheckPointIndex - 1, out TileCell result2)
				&& e.NewCell != result2)
				{
					accelerating = false;

					MoveToCell(routeCells[nextCheckPointIndex], Tween.TransitionType.Linear, Tween.EaseType.Out, Math.Abs(Subject.MoveDuration * 0.25f * nextPointGridDistance), false);

					return;
				}

			if (routeCells.Count > 1
				&& routeCells.TryToGetValue(nextCheckPointIndex - 1, out TileCell result3)
				&& e.NewCell == result3)
			{
				Decelerate();

				return;
			}

			checkPointReached = true;

			return;
		}

		private void Decelerate()
		{
			//if (accelerating) accelerating = false;

			//TileCell targetCell = routeCells.LastOrDefault();

			//if (routeCells.TryToGetValue(routeCellsIndex, out TileCell result))
			//{
			//	targetCell = result;

			//	MoveToCell(targetCell, Tween.TransitionType.Quad, Tween.EaseType.Out, Robot.MoveDuration * 0.75f, false);

			//	return;
			//}

			//DebugHelper.PrintError($"{Subject.Name}: There is no cell at {routeCellsIndex} in routecells for SmartMoveState");

			//MoveToCell(targetCell, Tween.TransitionType.Quad, Tween.EaseType.Out, Robot.MoveDuration * 0.75f, false);
		}

		public override void Invoke(CommandKey input)
		{
			//if (inProgress)
			//{
			//	interruptionRequested = true;

			//	newTarget = Robot.TileWorld.SelectedCell;

			//	DebugHelper.PrettyPrintVerbose($"InterruptionRequest recieved! New target: {newTarget}", ConsoleColor.DarkGray);

			//	return;
			//}

			//if (CalculatePath(Robot.TileWorld.SelectedCell))
			//{
			//	inProgress = true;

			//	EvaluateNextMove();
			//}
		}

		protected bool CalculatePath(TileCell targetCell)
		{
			//checkPointReached = false;

			//DebugHelper.PrettyPrintVerbose("Calculating path*...", ConsoleColor.DarkGray);

			//var currentCell = Robot.GetCurrentCell();

			//var list = Robot.TileWorld.AStar2D.GetIdPath(currentCell.AStarId, targetCell.AStarId);

			//if (list.Length == 0)
			//{
			//	DebugHelper.PrettyPrintVerbose("No avaible path found", ConsoleColor.DarkGray);

			//	Robot.SoundController.PlaySoundEffect(SoundController.CHIRP_SOUND);

			//	return false;
			//}

			//routeCellsIndex = 1;

			//var cellList = list.ToList().Select(id => Robot.TileWorld.GetCellByAStarId(id));

			//routeCells = cellList.ToList();

			//cellList.ToList().ForEach(cl =>
			//{
			//	DebugHelper.PrettyPrintVerbose($"Path step:{ cl}", ConsoleColor.DarkGray);
			//}

			//);

			return true;
		}

		protected void EvaluateNextMove()
		{
			TileCell targetCell;

			DebugHelper.PrettyPrintVerbose("Evaluating next move...", ConsoleColor.DarkGray);

			//if (routeCells.TryToGetValue(routeCellsIndex, out targetCell))
			//{
			//	var turnDirection = Robot.GetTurnDirection(targetCell.GridPosition);

			//	if (turnDirection == TurnDirection.Stay)
			//	{
			//		StartMove();

			//		return;
			//	}

			//	if (turnDirection == TurnDirection.Left)
			//	{
			//		TurnLeft();

			//		return;
			//	}

			//	if (turnDirection == TurnDirection.Right)
			//	{
			//		TurnRight();

			//		return;
			//	}
			//}

			//DebugHelper.PrettyPrintVerbose($"Invalid cellindex: {routeCellsIndex}", ConsoleColor.Red);

			FinishState();
		}

		protected virtual void StartMove()
		{
			//accelerating = true;

			//int checkPointIndex = routeCellsIndex;

			//DebugHelper.PrettyPrintVerbose($"Robot orientation: {Robot.Orientation}", ConsoleColor.DarkGray);

			//SetNextCheckpointIndex(checkPointIndex);

			//nextPointGridDistance = nextCheckPointIndex - routeCellsIndex;

			//DebugHelper.PrettyPrintVerbose($"NextCheckpointIndex: {nextCheckPointIndex}", ConsoleColor.DarkGray);

			//DebugHelper.PrettyPrintVerbose($"NextCheckpoint grid distance: {nextCheckPointIndex}", ConsoleColor.DarkGray);


			//if (routeCells.TryToGetValue(routeCellsIndex, out TileCell targetCell))
			//{
			//	DebugHelper.PrettyPrintVerbose($"Target gridPosition: {targetCell.GridPosition}", ConsoleColor.DarkGray);

			//	MoveToCell(targetCell, Tween.TransitionType.Cubic, Tween.EaseType.InOut, Robot.MoveDuration, true);
			//}

		}

		protected int SetNextCheckpointIndex(int currentCheckPointIndex)
		{
			//if (Robot.Orientation == WorldOrientation.SouthWest || Robot.Orientation == WorldOrientation.NorthEast)
			//{
			//	var xCoord = (int)routeCells[currentCheckPointIndex].GridPosition.x;

			//	while (routeCells.TryToGetValue(currentCheckPointIndex + 1, out TileCell result) && (int)result.GridPosition.x == xCoord)
			//	{
			//		currentCheckPointIndex++;
			//	}

			//	nextCheckPointIndex = currentCheckPointIndex;
			//}
			//else
			//{
			//	var yCoord = (int)routeCells[currentCheckPointIndex].GridPosition.y;

			//	while (routeCells.TryToGetValue(currentCheckPointIndex + 1, out TileCell result) && (int)result.GridPosition.y == yCoord)
			//	{
			//		currentCheckPointIndex++;
			//	}

			//	nextCheckPointIndex = currentCheckPointIndex;
			//}

			return nextCheckPointIndex;
		}


		protected virtual bool MoveToCell(TileCell targetCell, Tween.TransitionType transitionType, Tween.EaseType easeType, float duration, bool withSound)
		{
			//if (!Robot.ValidateMotionDirection(targetCell))
			//{
			//	DebugHelper.PrettyPrintVerbose($"{Robot.Name}: Invalid target detected: {targetCell}", ConsoleColor.Red);

			//	checkPointReached = true;

			//	return false;
			//}

			//Robot.TweenController.StopTweens();

			//var targetPosition = targetCell.WorldPositionOfCenter;

			//var interpolationParams = new InterpolateParams()
			//{
			//	Subject = Robot,

			//	Property = GodotProperties.position.ToString(),

			//	InitialValue = Robot.Position,

			//	FinalValue = targetPosition,

			//	TransitionType = transitionType,

			//	Duration = duration,

			//	EaseType = easeType,

			//	Delay = 0
			//};

			//Robot.TweenProperty(interpolationParams);

			//if (withSound) Robot.SoundController.PlaySoundEffect(SoundController.MOVE_SOUND);

			return true;
		}

		private void FinishState()
		{
			//DebugHelper.PrettyPrintVerbose($"{Robot.RobotName}: SmartMove finished..", ConsoleColor.DarkGray);

			//Reset();

			//OnStateFinished(new StateFinishedEventArgs() { NextState = StateEnum.Idle, Input = CommandKey.None });
		}
	}
}
