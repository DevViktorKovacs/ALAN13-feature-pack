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
    public class MoveState : State, ICharacterState
    {
        Dictionary<Occupant, Action<Vector2>> specialMoveActions = new Dictionary<Occupant, Action<Vector2>>();

        public MoveState(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler) : base(subject, eventHandler)
        {
            InstanceState = StateEnum.Move;

            TweenFinishedHandler = Parent_TweenFinished;

            EventTimerTimedOutHandler = Parent_TimerTimedOut;
        }

        public override void Invoke(CommandKey input)
        {
            if (input != CommandKey.None)
            {
                DebugHelper.Print($"Current state {CurrentState} Active: {Active}");

                Subject.StateController.QueueInput(input);

                return;
            }

            Subject.UpdateTargetCell();

            var targetPosition = Subject.TargetCell.WorldPositionOfCenter;

            Action<Vector2> moveAction;

            var isSpecialCase = specialMoveActions.TryGetValue(Subject.TargetCell.Occupant, out moveAction);

            if (isSpecialCase)
            {
                moveAction(targetPosition);

                return;
            }

            NextState = StateEnum.Idle;

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
