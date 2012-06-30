using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSpeex;
using Poker.Client.Audio;
using UnityEngine;

namespace Poker.Client.Behaviours
{
    public class PlayerVoiceBehaviour : MonoBehaviour
    {
        private readonly AudioInputRecorder recorder = new AudioInputRecorder(44100);
        private int                         sampleFrequency;
        private IAudioEncoder               audioEncoder;
        private IAudioDecoder               audioDecoder;
        private Stream                      audioStream;
        private List<float>                 decodingBuffer; 

        public void Awake()
        {
            StartCoroutine(InitializeMicrophone());
        }

        public void Update()
        {
            recorder.Update();
        }

        public void OnDestroy()
        {
            audioEncoder.Close();
        }

        public void PlayVoiceOutput(byte[] data,int count)
        {
            audioDecoder.Write(data,count);
        }

        private IEnumerator InitializeMicrophone()
        {
            Debug.Log("Trying to initialize microphone");

            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Debug.Log("Microphone access was granted");

                foreach( var device in recorder.Devices)
                {
                    Debug.Log(device);
                }

                sampleFrequency = recorder.Start(recorder.Devices.First(), (data,count) =>
                {
                    if ( audioEncoder == null )
                    {
                        audioEncoder    = new SpeexAudioEncoder(false,BandMode.Wide,false,10);
                        audioDecoder    = new SpeexAudioDecoder(BandMode.Wide);
                        decodingBuffer  = new List<float>();

                        audioStream  = new EventStream((buffer,bufferOffset,bufferCount) =>
                        {
                            var playerNetworkBehaviour = GetComponent<PlayerNetworkBehaviour>(); 
                            if ( playerNetworkBehaviour != null )
                            {
                                playerNetworkBehaviour.SendVoiceInput(buffer,bufferOffset,bufferCount);
                            }
                        });

                        audioEncoder.Open(audioStream,1,sampleFrequency);
                        audioDecoder.Open((buffer,bufferCount) =>
                        {
                            decodingBuffer.AddRange(buffer.Take(bufferCount));

                            if ( decodingBuffer.Count > sampleFrequency / 4)
                            {
                                if ( !audio.isPlaying )
                                {
                                    audio.clip = AudioClip.Create("VoiceOutput", decodingBuffer.Count, 1, sampleFrequency, false, false);
                                    audio.clip.SetData(decodingBuffer.ToArray(),0);
                                    decodingBuffer.Clear();
                                    audio.Play();
                                }
                            }
                        });
                    }

                    audioEncoder.Write(data,count);
                });
            }
        }
    }
}