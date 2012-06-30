using System;

namespace Poker.Client.Audio
{
    public interface IAudioDecoder
    {
        void Open(Action<float[], int> decodeCallback);
        void Write(byte[] input, int count);
    }
}