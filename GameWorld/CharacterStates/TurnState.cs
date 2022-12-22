using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
    public class TurnState : State, ICharacterState
    {
        protected AnimationKeys AnimationName = AnimationKeys.TurnLeft;

        public TurnState(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler) : base(subject, eventHandler)
        {
            InstanceState = StateEnum.Turn;

            AnimationFinishedHandler = Parent_AnimationFinished;
        }

        private void Parent_AnimationFinished(object sender, AnimationFinishedEventArgs e)
        {
            if (!Active) return;

            if (e.AnimiationID != AnimationId || AnimationId == 0) return;

            if (e.AnimationName.Contains(keys[AnimationName]))
            {
                var sfe = new StateFinishedEventArgs() { NextState = NextState, Input = CommandKey.None };

                OnStateFinished(sfe);
            }
        }

        public override void Invoke(CommandKey input)
        {
            NextState = StateEnum.Idle;

            AnimationId = 0;

            if (input == CommandKey.TurnLeft)
            {
                AnimationName = AnimationKeys.TurnLeft;

                Turn(true);

                return;
            }

            if (input == CommandKey.TurnRight)
            {
                AnimationName = AnimationKeys.TurnRight;

                Turn(false);

                return;
            }
        }

        private void Turn(bool left)
        {
            var currentOrientation = Subject.Orientation;

            Subject.Orientation = left ? StaticData.TurnOrientationLeft(Subject.Orientation) : StaticData.TurnOrientationRight(Subject.Orientation);

            Subject.UpdateDirection();

            AnimationId = Subject.PlayOrientationSpecificAnimation(keys[AnimationName], currentOrientation);
        }
    }
}
