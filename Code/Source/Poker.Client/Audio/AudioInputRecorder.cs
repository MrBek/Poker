using System;
using System.Collections.Generic;
using System.ComponentModel;
using Poker.Client.Conversion;
using UnityEngine;

namespace Poker.Client.Audio
{
    public class AudioInputRecorder
    {
        private const int BufferLengthInSeconds = 3;

        private readonly int                defaultSampleFrequency;

        private AudioInputDeviceDescription currentRecordingDevice;
        private Action<float[],int>         bufferCallback;
        private int                         sampleFrequency;
        private int                         bufferLengthInSamples;

        private float[]                     readBuffer;
        private AudioClip                   recordingBuffer;
        private int                         currentSamplePosition;

        public IEnumerable<AudioInputDeviceDescription> Devices
        {
            get
            {
                foreach( var deviceName in Microphone.devices)
                {
                    var minFrequency = 0;
                    var maxFrequency = 0;

                    Microphone.GetDeviceCaps(deviceName,out minFrequency,out maxFrequency);

                    yield return new AudioInputDeviceDescription(deviceName,minFrequency,maxFrequency);
                }
            }
        }

        public int Start(AudioInputDeviceDescription device,Action<float[],int> bufferCallback )
        {
            if ( device == null )
            {
                throw new ArgumentNullException("device");
            }

            if ( bufferCallback == null )
            {
                throw new ArgumentNullException("bufferCallback");
            }

            Stop();

            currentRecordingDevice  = device;
            this.bufferCallback     = bufferCallback;
        
            // Select sample frequency:
            sampleFrequency         = device.MaxFrequency > 0 ? device.MaxFrequency : defaultSampleFrequency;
            bufferLengthInSamples   = sampleFrequency*BufferLengthInSeconds;
            currentSamplePosition   = 0;

            readBuffer              = new float[bufferLengthInSamples];
            recordingBuffer         = Microphone.Start(currentRecordingDevice.Name, false, BufferLengthInSeconds, sampleFrequency);

            return sampleFrequency;
        }

        public void Update()
        {
            if ( currentRecordingDevice != null )
            {
                var newSamplePosition = Microphone.GetPosition(currentRecordingDevice.Name);
                var availableSampleCount = newSamplePosition - currentSamplePosition;

                if (availableSampleCount > 0)
                {
                    recordingBuffer.GetData(readBuffer,currentSamplePosition);
                    bufferCallback(readBuffer,availableSampleCount);
                }

                currentSamplePosition = newSamplePosition;

                if ( currentSamplePosition + sampleFrequency > bufferLengthInSamples)
                {
                    recordingBuffer         = Microphone.Start(currentRecordingDevice.Name, false, BufferLengthInSeconds, sampleFrequency);
                    currentSamplePosition   = 0;
                }
            }
        }

        public void Stop()
        {
            if ( currentRecordingDevice != null )
            {
                Microphone.End(currentRecordingDevice.Name);

                currentRecordingDevice = null;
            }
        }

        public AudioInputRecorder(int defaultSampleFrequency)
        {
            this.defaultSampleFrequency = defaultSampleFrequency;
        }
    }
}