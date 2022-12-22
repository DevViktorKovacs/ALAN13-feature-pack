using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
    public class MoveState : State, ICharacterState
    {
        Dictionary<Occupant, Action<Vector2>> specialMoveActions = new Dictionary<Occupant, Action<Vector2>>();

        public MoveState(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler) : base(subject, eventHandler)
        {
            InstanceState = StateEnum.Move;

            TweenFinishedHandler = Parent_TweenFinished;

            EventTimerTimedOutHandler = Parent_TimerTimedOut;

            NextState = StateEnum.Idle;
        }

        public override void Invoke(CommandKey input)
        {
            if (input != CommandKey.None)
            {
                DebugHelper.PrettyPrintVerbose($"Current state {CurrentState} Active: {Active}");

                Subject.StateController.QueueInput(input);

                return;
            }

            Subject.UpdateTargetCell();

            var targetPosition = Subject.TargetCell.WorldPositionOfCenter;

            DebugHelper.PrettyPrintVerbose($"Current position: {Subject.Position}");

            DebugHelper.PrettyPrintVerbose($"Move target cell: {Subject.TargetCell.GridPosition}, position: {targetPosition}");

            Action<Vector2> moveAction;

            var isSpecialCase = specialMoveActions.TryGetValue(Subject.TargetCell.Occupant, out moveAction);

            if (isSpecialCase)
            {
                moveAction(targetPosition);

                return;
            }

            Subject.TweenProperty(GodotProperties.position.ToString(), Subject.Position, targetPosition);

            return;
        }

        private void Parent_TimerTimedOut(object sender, EventArgs e)
        {
            if (!Active) return;

            Subject.StateController.StopStateTimer();

            OnStateFinished(GetEventArgs());
        }

        private void Parent_TweenFinished(object sender, EventArgs e)
        {
            if (!Active) return;

            OnStateFinished(GetEventArgs());
        }

        private StateFinishedEventArgs GetEventArgs()
        {
            return new StateFinishedEventArgs() { NextState = NextState, Input = CommandKey.None };
        }
    }
}
