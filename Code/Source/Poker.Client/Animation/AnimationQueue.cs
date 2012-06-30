using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Client.Animation
{
    public class AnimationQueueEntry
    {
        public string               Name        { get; private set; }
        public float                StartTime   { get; private set; }
        public AnimationPlayMode    PlayMode    { get; private set; }
        public float                BlendWeight { get; private set; }
        public bool                 OnlyIfNotPlaying { get; private set; }

        internal AnimationQueueEntry(string name,float startTime,AnimationPlayMode playMode,float blendWeight,bool onlyIfNotPlaying)
        {
            this.Name               = name;
            this.StartTime          = startTime;
            this.PlayMode           = playMode;
            this.BlendWeight        = blendWeight;
            this.OnlyIfNotPlaying   = onlyIfNotPlaying;
        }
    }

    public class AnimationQueue
    {
        private readonly UnityEngine.Animation      animation;
        private readonly List<AnimationQueueEntry>  entries = new List<AnimationQueueEntry>();
        private readonly Action<string, float>      animationStartedCallback;

        public void Enqueue(string name,float delay)
        {
            Enqueue(name, delay, AnimationPlayMode.Mix);
        }

        public void Enqueue(string name,float delay,AnimationPlayMode playMode)
        {
            Enqueue(name, delay, playMode, 1.0f);
        }

        public void Enqueue(string name,float delay,AnimationPlayMode playMode,float blendWeight)
        {
            Enqueue(name, delay, playMode, blendWeight, true);
        }

        public void Enqueue(string name,float delay,AnimationPlayMode playMode,float blendWeight,bool onlyIfNotPlaying)
        {
            entries.Add(new AnimationQueueEntry(name,Time.time + delay,playMode,blendWeight,onlyIfNotPlaying));
        }

        public void Update()
        {
            var currentTime = Time.time;

            foreach( var entry in entries.ToArray())
            {
                if ( entry.StartTime <= currentTime )
                {
                    entries.Remove(entry);

                    if ( entry.OnlyIfNotPlaying )
                    {
                        if ( animation.IsPlaying(entry.Name) )
                        {
                            continue;
                        }
                    }

                    if ( entry.BlendWeight > 0.0f )
                    {
                        animation.Blend(entry.Name,entry.BlendWeight);
                    }
                    else
                    {
                        animation.Play(entry.Name, entry.PlayMode);
                    }

                    animationStartedCallback(entry.Name, animation.GetClip(entry.Name).length);
                }
            }
        }

        public AnimationQueue(UnityEngine.Animation animation,Action<string,float> animationStartedCallback)
        {
            this.animation                  = animation;
            this.animationStartedCallback   = animationStartedCallback;
        }
    }
}