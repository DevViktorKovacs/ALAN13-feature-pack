using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.Utility
{
    public class AnimationFactory
    {
        public List<AnimationRecord> AnimationRecords { get; set; }

        public List<AnimationRecord> ActiveAnimations { get; set; }

        long idCounter = 1;

        public AnimationFactory()
        {
            AnimationRecords = new List<AnimationRecord>();

            ActiveAnimations = new List<AnimationRecord>();
        }

        public void Reset()
        {
            idCounter = 1;

            AnimationRecords.Clear();

            ActiveAnimations.Clear();
        }

        public AnimationRecord PopRecord(string animationName)
        {
            AnimationRecord result = null;

            for (int i = ActiveAnimations.Count - 1; i >= 0; i--)
            {
                if (ActiveAnimations[i].AnimationName == animationName)

                    result = ActiveAnimations[i];

                ActiveAnimations.RemoveAt(i);
            }

            return result;
        }

        public long PushRecord(AnimatedSprite animatedSprite, string animation)
        {
            var newRecord = new AnimationRecord()
            {
                AnimationName = animation,

                FrameCount = animatedSprite.Frames.GetFrameCount(animation),

                CurrentFrame = animatedSprite.Frame,

                ID = idCounter++

            };

            ActiveAnimations.Add(newRecord);

            AnimationRecords.Add(newRecord);

            return newRecord.ID;
        }
    }
}
