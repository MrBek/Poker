using System;
using System.IO;
using NSpeex;
using Poker.Client.Conversion;
using UnityEngine;

namespace Poker.Client.Audio
{
    public class WavAudioEncoder : AudioEncoderBase
    {
        private PcmWaveWriter waveWriter;

        protected override void OnOpen(System.IO.Stream targetStream, int channelCount, int sampleFrequency)
        {
            waveWriter = new PcmWaveWriter(sampleFrequency, channelCount);
            waveWriter.Open(targetStream);
            waveWriter.WriteHeader("Unity3d");
        }

        protected override void OnClose()
        {
            waveWriter.Close();
        }

        protected override void OnWrite(byte[] buffer,int count)
        {
            waveWriter.WritePacket(buffer,0,count);
        }
    }
}