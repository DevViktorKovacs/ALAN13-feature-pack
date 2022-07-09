using ALAN13featurepack.GameWorld.CharacterStates;
using System;


namespace ALAN13featurepack.Interfaces
{
    public interface ICharacterState
    {
        void Invoke(CommandKey input);

        StateEnum CurrentState { get; }

        bool Active { get; set; }

        void Reset();

        event EventHandler<StateFinishedEventArgs> StateFinished;

        Action<object, AnimationFinishedEventArgs> AnimationFinishedHandler { get; set; }

        Action<object, EventArgs> TweenFinishedHandler { get; set; }

        Action<object, EventArgs> EventTimerTimedOutHandler { get; set; }

        GameCharacter Subject { get; set; }
    }
}
