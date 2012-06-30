using System;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerAnimationBehaviour : MonoBehaviour
    {
        public float MinDelayBetweenAnimations = 1.0f;
        public float MaxDelayBetweenAnimations = 5.0f;

        private System.Random random;

        private float   currentDelay;
        private float   delay;
        private bool    isDelaying;

        public void Awake()
        {
            random = new System.Random(name.GetHashCode());
            animation.Play("SittingIdle");
        }

        public void StartSpeechAnimation(float delayInSeconds)
        {
            Debug.Log("Speech triggered");

            if ( !animation.IsPlaying("SittingTalking"))
            {
                animation.Blend("SittingTalking");
            }
        }

        public void StopSpeechAnimation()
        {
            if ( animation.IsPlaying("SittingTalking"))
            {
                animation.Stop("SittingTalking");
                animation.Play("SittingIdle", AnimationPlayMode.Mix);
            }
        }

        public void Update()
        {
            if ( !animation.IsPlaying("SittingIdle") )
            {
                if (!isDelaying)
                {
                    isDelaying      = true;
                    delay           = MinDelayBetweenAnimations + (float) random.NextDouble()*(MaxDelayBetweenAnimations - MinDelayBetweenAnimations);
                    currentDelay    = 0.0f;
                }
                else
                {
                    currentDelay += Time.deltaTime;

                    if ( currentDelay > delay )
                    {
                        animation.Play("SittingIdle",AnimationPlayMode.Mix);
                        isDelaying = false;
                    }
                }
            }
        }
    }
}