using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
    public class State : ICharacterState
    {
        protected StateEnum InstanceState { get; set; }

        protected StateEnum DefaultNextState { get; set; }

        public StateEnum NextState { get; set; }

        public StateEnum CurrentState { get { return InstanceState; } }

        protected Dictionary<AnimationKeys, string> keys = StaticData.AnimationData;

        protected long AnimationId { get; set; }

        public bool Active { get; set; }

        public Action<object, AnimationFinishedEventArgs> AnimationFinishedHandler { get; set; }
        public Action<object, EventArgs> TweenFinishedHandler { get; set; }
        public Action<object, EventArgs> EventTimerTimedOutHandler { get; set; }
        public GameCharacter Subject { get; set; }

        public event EventHandler<StateFinishedEventArgs> StateFinished;

        public State(GameCharacter subject, EventHandler<StateFinishedEventArgs> eventHandler)
        {
            Subject = subject;

            Active = false;

            StateFinished += eventHandler;
        }
        public virtual void Invoke(CommandKey input)
        {
            DebugHelper.Print("Operation not implemented");
        }

        public virtual void Reset()
        {
            NextState = DefaultNextState;
        }

        public virtual void OnStateFinished(StateFinishedEventArgs e)
        {
            StateFinished?.Invoke(this, e);
        }
    }
}
