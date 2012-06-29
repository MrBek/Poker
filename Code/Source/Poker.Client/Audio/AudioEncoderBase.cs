using Poker.Client.Conversion;

namespace Poker.Client.Audio
{
    public abstract class AudioEncoderBase : IAudioEncoder
    {
        private short[] shortBuffer;
        private byte[]  byteBuffer;

        public void Open(System.IO.Stream targetStream, int channelCount, int sampleFrequency)
        {
            OnOpen(targetStream,channelCount,sampleFrequency);
        }

        public void Close()
        {
            OnClose();
        }

        public void Write(float[] samples, int count)
        {
            if (shortBuffer == null || shortBuffer.Length < count)
            {
                shortBuffer = new short[count];
            }

            if (byteBuffer == null || byteBuffer.Length < count * 2)
            {
                byteBuffer = new byte[count * 2];
            }

            ArrayConverters.FloatToShort(samples, count, shortBuffer);
            ArrayConverters.GetBytes(shortBuffer, count, byteBuffer);

            OnWrite(byteBuffer, count*2);
        }

        protected abstract void OnOpen(System.IO.Stream targetStream, int channelCount, int sampleFrequency);
        protected abstract void OnClose();
        protected abstract void OnWrite(byte[] buffer, int count);
    }
}