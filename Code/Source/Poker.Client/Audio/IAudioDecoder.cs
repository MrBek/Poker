using System;

namespace Poker.Client.Audio
{
    public interface IAudioDecoder
    {
        int SampleRate { get; }

        void Open(Action<float[], int> decodeCallback);
        void Write(byte[] input, int count);
    }
}