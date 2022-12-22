using ALAN13featurepack.GameWorld.CharacterStates;
using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld
{
    public class StateController
    {
        protected ICharacterState currentState;

        protected int commandID = 0;

        protected float commandDuration = 0.85f;

        protected bool commandTimerIsRunning = false;

        static int inputBuffer = 3;

        public Queue<CommandKey> InputQueue;

        GameCharacter parent;

        private Timer stateEventTimer;

        protected float DefaultStateTimerWait = 0.5f;

        protected Timer CommandTimer;

        public Action<object, AnimationFinishedEventArgs> AnimationFinishedAction;

        public Action<object, EventArgs> TweenFinishedAction;

        public Action<object, EventArgs> EventTimerTimedOutAction;

        static List<StateEnum> immidiateStates = new List<StateEnum>() { };

        public Dictionary<StateEnum, ICharacterState> StateDictionary { get; set; }

        public StateController(GameCharacter subject, List<Timer> timers)
        {
            parent = subject;

            StateDictionary = new Dictionary<StateEnum, ICharacterState>();

            InputQueue = new Queue<CommandKey>();

            StateDictionary = new Dictionary<StateEnum, ICharacterState>();

            StateDictionary.Add(StateEnum.Idle, new IdleState(parent, State_StateFinished));

            StateDictionary.Add(StateEnum.Move, new MoveState(parent, State_StateFinished));

            currentState = StateDictionary[StateEnum.Idle];

            currentState.Active = true;

            stateEventTimer = timers.First();

            CommandTimer = timers.Last();
        }

        public void IssueCommand(CommandKey input, object caller)
        {
            if (commandTimerIsRunning && !immidiateStates.Contains(currentState.CurrentState))
            {
                InputQueue.Enqueue(input);

                DebugHelper.PrettyPrintVerbose($"{parent.CharacterName}: Enquiuing Command: {input} on {currentState.CurrentState} by {caller.GetType()}!", ConsoleColor.DarkYellow);
            }
            else
            {
                CommandTimer.Stop();

                commandTimerIsRunning = false;

                DebugHelper.PrettyPrintVerbose($"{parent.CharacterName}: Invoking Command: {input} on {currentState.CurrentState} by {caller}!", ConsoleColor.DarkYellow);

                InvokeCommand(input);
            }
        }

        private void InvokeCommand(CommandKey input)
        {
            //if (parent.ProgramExecutionMode && timeConsumingStates.Contains(currentRobotState.CurrentState))
            //{
            //    CommandTimer.WaitTime = commandDuration / parent.AnimationSpeed;

            //    CommandTimer.Start();

            //    commandTimerIsRunning = true;
            //}

            currentState.Invoke(input);
        }

        public void StopStateTimer()
        {
            stateEventTimer.Stop();
        }

        public void QueueInput(CommandKey input)
        {
            if (InputQueue.Count < inputBuffer)
            {
                InputQueue.Enqueue(input);

                DebugHelper.Print($"Input: {input} is queued.");
            }
            else
            {
                DebugHelper.Print($"Input queue is full, input: {input} will not be executed.");
            }
        }

        public void State_StateFinished(object sender, StateFinishedEventArgs e)
        {
            DebugHelper.PrettyPrintVerbose($"{parent.CharacterName} Changing State - from:{currentState.CurrentState} to:{e.NextState}", ConsoleColor.DarkYellow);

            SwitchState(e);
        }

        public void SwitchState(StateFinishedEventArgs e)
        {
            var previousState = currentState.CurrentState;

            currentState.Active = false;
            currentState = StateDictionary[e.NextState];
            currentState.Active = true;

            AnimationFinishedAction = currentState.AnimationFinishedHandler;

            TweenFinishedAction = currentState.TweenFinishedHandler;

            EventTimerTimedOutAction = currentState.EventTimerTimedOutHandler;

            var cfe = new CommandFinishedEventArgs()
            {
                PreviousState = previousState,
                NewState = e.NextState,
                CommandID = commandID++,
                Handled = false
            };

            DebugHelper.PrettyPrintVerbose($"{parent.CharacterName}: New state: {e.NextState}", ConsoleColor.DarkYellow);

            if (e.NextState == StateEnum.Idle)
            {
                parent.OnCommandFinished(cfe);

                return;
            }

            if (e.Input != CommandKey.Skip)
            {
                IssueCommand(e.Input, this);

                return;
            }
        }
        public void SwitchState(StateEnum nextState)
        {
            var e = new StateFinishedEventArgs() { Input = CommandKey.Skip, NextState = nextState };

            SwitchState(e);
        }
    }
}
