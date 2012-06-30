using System.Collections.Generic;
using System.Linq;
using NSpeex;
using Poker.Client.Audio;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerVoiceOutputBehaviour : MonoBehaviour 
    {
        private readonly List<float>    decoderBuffer = new List<float>();
        private IAudioDecoder           audioDecoder;

        public float SpeechVolumneThreshold = 0.3f;
    
        public void PlayVoiceOutput(byte[] compressedBuffer,int count,int sampleFrequency)
        {
            if ( audioDecoder == null )
            {
                audioDecoder = new SpeexAudioDecoder(BandMode.Wide);
                audioDecoder.Open((buffer, bufferCount) =>
                {
                    decoderBuffer.AddRange(buffer.Take(bufferCount));

                    if (decoderBuffer.Count > audioDecoder.SampleRate / 4)
                    {
                        if (!audio.isPlaying)
                        {
                            var delayInSeconds = 0.0f;

                            var currentAudioClip = audio.clip;
                            if ( currentAudioClip != null )
                            {
                                delayInSeconds = currentAudioClip.samples/(float)sampleFrequency;   
                            }

                            audio.clip = AudioClip.Create("VoiceOutput", decoderBuffer.Count, 1, sampleFrequency, true, false);
                            audio.clip.SetData(decoderBuffer.ToArray(), 0);

                            var playerAnimationBehaviour = GetComponent<PlayerAnimationBehaviour>();
                            if (playerAnimationBehaviour != null)
                            {
                                var maxVolume = decoderBuffer.Max(b => b);

                                if (maxVolume > SpeechVolumneThreshold)
                                {
                                    playerAnimationBehaviour.StartSpeechAnimation(0.0f);
                                }
                                else
                                {
                                    playerAnimationBehaviour.StopSpeechAnimation();
                                }
                            }

                            decoderBuffer.Clear();
                            audio.Play();
                        }
                    }
                });
            }

            audioDecoder.Write(compressedBuffer, count);
        }
    }
}