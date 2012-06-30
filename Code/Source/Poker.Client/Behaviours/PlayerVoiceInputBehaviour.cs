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
    public class PlayerVoiceInputBehaviour : MonoBehaviour
    {
        private readonly AudioInputRecorder recorder = new AudioInputRecorder(44100);
        private int                         sampleFrequency;
        private IAudioEncoder               audioEncoder;
        private Stream                      audioStream;
      
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
                        audioStream  = new EventStream((buffer,bufferOffset,bufferCount) =>
                        {
                            var playerNetworkBehaviour = GetComponent<PlayerNetworkBehaviour>(); 
                            if ( playerNetworkBehaviour != null )
                            {
                                playerNetworkBehaviour.SendVoiceInput(buffer,bufferOffset,bufferCount,sampleFrequency);
                            }
                        });

                        audioEncoder.Open(audioStream,1,sampleFrequency);
                    }

                    audioEncoder.Write(data,count);
                });
            }
        }
    }
}