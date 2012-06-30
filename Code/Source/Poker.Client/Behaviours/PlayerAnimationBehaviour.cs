using System;
using Poker.Client.Animation;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerAnimationBehaviour : MonoBehaviour
    {
        public float MinDelayBetweenAnimations = 1.0f;
        public float MaxDelayBetweenAnimations = 5.0f;

        private System.Random   random;
        private AnimationQueue  animationQueue;

        private float   currentDelay;
        private float   delay;
        private bool    isDelaying;

        public void Awake()
        {
            random = new System.Random(name.GetHashCode());
            animationQueue = new AnimationQueue(animation,(animationName,length) =>
            {
                if ( animationName == "SittingIdle")
                {
                    var delay = MinDelayBetweenAnimations + (float)random.NextDouble() * (MaxDelayBetweenAnimations - MinDelayBetweenAnimations);
                    Debug.Log(string.Format("Play idle for {0} in {1}",name,delay + length));
                    animationQueue.Enqueue("SittingIdle",delay + length,AnimationPlayMode.Mix,0.5f,false);
                }
            }); 
        
            animationQueue.Enqueue("SittingIdle",0.0f,AnimationPlayMode.Mix,0.5f);
        }

        public void StartSpeechAnimation(float delayInSeconds)
        {
            Debug.Log("Speech triggered");
            animationQueue.Enqueue("SittingTalking",delayInSeconds,AnimationPlayMode.Mix,0.5f);
        }

        public void StopSpeechAnimation()
        {
            if ( animation.IsPlaying("SittingTalking"))
            {
                animation.Stop("SittingTalking");
            }
        }

        public void Update()
        {
            animationQueue.Update();
        }
    }
}