using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
    public class IdleState : State, ICharacterState
    {
        public IdleState(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler) : base(subject, eventHandler)
        {
            InstanceState = StateEnum.Idle;
        }

        public override void Invoke(CommandKey input)
        {
            if (input == CommandKey.None)
            {
                if (Subject.StateController.InputQueue.Count == 0)
                {
                    Subject.PlayOrientationSpecificAnimation(keys[AnimationKeys.Default], Subject.Orientation);
                    return;
                }
                else
                {
                    Invoke(Subject.StateController.InputQueue.Dequeue());
                    return;
                }
            }

            if (input == CommandKey.TurnLeft || input == CommandKey.TurnRight)
            {
                var e = new StateFinishedEventArgs() { NextState = StateEnum.Turn, Input = input };
                OnStateFinished(e);
                return;
            }

            if (input == CommandKey.MoveForward)
            {
                var e = new StateFinishedEventArgs() { NextState = StateEnum.Move, Input = CommandKey.None };
                OnStateFinished(e);
                return;
            }


            if (input == CommandKey.SmartMove)
            {
                var e = new StateFinishedEventArgs() { NextState = StateEnum.SmartMove, Input = CommandKey.None };
                OnStateFinished(e);
                return;
            }
        }
    }
}
