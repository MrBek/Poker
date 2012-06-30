using System;
using NSpeex;
using Poker.Client.Conversion;

namespace Poker.Client.Audio
{
    public class SpeexAudioDecoder : IAudioDecoder
    {
        private readonly BandMode       bandMode;

        private SpeexDecoder            decoder;
        
        private SpeexJitterBuffer       jitterBuffer;
        private short[]                 readBuffer;
        private float[]                 writeBuffer;

        private Action<float[], int>    decodeCallback;

        public int SampleRate
        {
            get { return decoder.SampleRate; }
        }

        public void Open(Action<float[],int> decodeCallback)
        {
            this.decodeCallback = decodeCallback;
            decoder = new SpeexDecoder(bandMode,true);
        }

        public void Write(byte[] data,int count)
        {
            if ( readBuffer == null || readBuffer.Length < count * 10)
            {
                readBuffer = new short[count * 10];
            }

            if ( writeBuffer == null || writeBuffer.Length < count * 10)
            {
                writeBuffer = new float[count * 10];
            }

            var samplesDecoded = decoder.Decode(data, 0, count, readBuffer, 0, false);

            if (samplesDecoded > 0)
            {
                ArrayConverters.ShortToFloat(readBuffer, samplesDecoded, writeBuffer);
                decodeCallback(writeBuffer, samplesDecoded);
            }
        }

        public SpeexAudioDecoder(BandMode bandMode)
        {
            this.bandMode = bandMode;
        }
    }
}