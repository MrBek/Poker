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
        private IAudioEncoder               wavEncoder;
        private IAudioEncoder               speexEncoder;
        private Stream                      wavStream;
        private Stream                      speexStream;
        private int                         sampleFrequency;

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
            Debug.Log("Destroy");

            wavEncoder.Close();
            speexEncoder.Close();
           
            wavStream.Dispose();
            speexStream.Dispose();
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
                    if ( wavEncoder == null )
                    {
                        var wavData = CreateEncoder("test.wav", () => new WavAudioEncoder(), 1, sampleFrequency);
                        var speexData = CreateEncoder("test.ogg", () => new SpeexAudioEncoder(true, BandMode.UltraWide, false, 10), 1, sampleFrequency);

                        wavEncoder = wavData.Key;
                        wavStream = wavData.Value;

                        speexEncoder = speexData.Key;
                        speexStream = speexData.Value;
                    }

                    wavEncoder.Write(data,count);
                    speexEncoder.Write(data,count);
                });

                Debug.Log("Sample frequency: " + sampleFrequency);
            }
        }

        private KeyValuePair<IAudioEncoder,Stream>  CreateEncoder(string filename,Func<IAudioEncoder> creator,int channelCount,int sampleRate)
        {
            if ( File.Exists(filename))
            {
                File.Delete(filename);
            }

            var stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            var encoder = creator();

            encoder.Open(stream, channelCount, sampleRate);

            return new KeyValuePair<IAudioEncoder, Stream>(encoder,stream);
        }
    }
}