using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ALAN13featurepack.Utility.ExtensionMethods;
using static Godot.Tween;

namespace ALAN13featurepack.Utility
{
    public class TweenController
    {
        int durationLowEnd;

        int durationHighEnd;

        Random random;

        Tween tween;

        public TransitionType TransitionType { get; set; }

        public EaseType EaseType { get; set; }

        public float Duration { get; set; }

        public bool Randomize { get; set; }

        public Godot.Object Subject { get; set; }

        public NodePath NodePath { get; set; }

        public TweenController(int DurationHighEnd, int DurationLowEnd, Tween tween, float duration)
        {
            durationHighEnd = DurationHighEnd;

            durationLowEnd = DurationLowEnd;

            random = new Random();

            this.tween = tween;

            Randomize = true;

            TransitionType = TransitionType.Cubic;

            EaseType = EaseType.InOut;

            Duration = duration;
        }

        public void InterpolateProperty(Godot.Object @object, NodePath property, object initialVal, object finalVal, float duration = -1, EaseType easeType = EaseType.InOut)
        {
            var rnd = random.Next(6, 10);

            int transitionTypeInt = rnd % 8;

            var transitionType = Randomize ? (TransitionType)transitionTypeInt : TransitionType.Cubic;

            var rndDuration = Randomize ? (float)random.Next(durationLowEnd, durationHighEnd) / 100 : Duration;

            tween.InterpolateProperty(@object, property, initialVal, finalVal, duration < 0 ? rndDuration : duration, transitionType, easeType);

            tween.Start();
        }

        public void InterpolateProperty(InterpolateParams interpolateParams)
        {
            tween.InterpolateProperty(interpolateParams);

            tween.Start();
        }

        public void StopTweens()
        {
            DebugHelper.PrettyPrintVerbose($"{(Subject as GameCharacter).CharacterName}: Stopping all tweens!", ConsoleColor.DarkCyan);

            tween.StopAll();

            tween.RemoveAll();
        }

        public void Interpolate(object initialVal, object finalVal)
        {
            tween.InterpolateProperty(Subject, NodePath, initialVal, finalVal, Duration, TransitionType, EaseType);

            tween.Start();
        }

        public TransitionType NextTransitionType()
        {
            var currentTransitionTpyeInt = (int)TransitionType;

            currentTransitionTpyeInt = (currentTransitionTpyeInt + 1) % 11;

            TransitionType = (TransitionType)currentTransitionTpyeInt;

            return TransitionType;
        }

        public TransitionType PreviousTransitionType()
        {
            var currentTransitionTpyeInt = (int)TransitionType;

            currentTransitionTpyeInt--;

            if (currentTransitionTpyeInt < 0) currentTransitionTpyeInt = 10;

            TransitionType = (TransitionType)currentTransitionTpyeInt;

            return TransitionType;
        }

        public EaseType NextEaseType()
        {
            var currentETInt = (int)EaseType;

            currentETInt = (currentETInt + 1) % 4;

            EaseType = (EaseType)currentETInt;

            return EaseType;
        }

        public EaseType PreviousEaseType()
        {
            var currentETInt = (int)EaseType;

            currentETInt--;

            if (currentETInt < 0) currentETInt = 3;

            EaseType = (EaseType)currentETInt;

            return EaseType;
        }
    }
}
