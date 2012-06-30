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
        }

        public void TriggerSpeechAnimation()
        {
            Debug.Log("Speech triggered");
        }

        public void Update()
        {
            if ( !animation.isPlaying )
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
                        animation.Play();
                        isDelaying = false;
                    }
                }
            }
        }
    }
}