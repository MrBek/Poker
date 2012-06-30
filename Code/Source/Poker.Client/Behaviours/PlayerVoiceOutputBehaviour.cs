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
                            audio.clip = AudioClip.Create("VoiceOutput", decoderBuffer.Count, 1, sampleFrequency, true, false);
                            audio.clip.SetData(decoderBuffer.ToArray(), 0);

                            var averageVolume = decoderBuffer.Average(b => b);
                            Debug.Log("Volume: " + averageVolume);
                            if (averageVolume > 0.4f)
                            {
                                var playerAnimationBehaviour = GetComponent<PlayerAnimationBehaviour>();
                                if (playerAnimationBehaviour != null)
                                {
                                    playerAnimationBehaviour.TriggerSpeechAnimation();
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