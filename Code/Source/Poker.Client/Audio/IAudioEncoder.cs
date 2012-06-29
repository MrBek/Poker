using System;
using System.IO;

namespace Poker.Client.Audio
{
    public interface IAudioEncoder
    {
        void Open(Stream targetStream,int channelCount,int sampleFrequency);
        void Close();

        void Write(float[] samples,int count);
    }
}