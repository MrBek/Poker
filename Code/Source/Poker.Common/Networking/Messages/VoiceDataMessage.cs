namespace Poker.Common.Networking.Messages
{
    public class VoiceDataMessage : NetworkMessageBase
    {
        public byte[]   Data            { get; set; }
        public int      SampleFrequency { get; set; }

        protected override void OnRead(System.IO.BinaryReader reader)
        {
            var dataLength = reader.ReadInt32();

            Data = new byte[dataLength];
            Data = reader.ReadBytes(dataLength);
            SampleFrequency = reader.ReadInt32();
        }

        protected override void OnWrite(System.IO.BinaryWriter writer)
        {
            writer.Write(Data.Length);
            writer.Write(Data,0,Data.Length);
            writer.Write(SampleFrequency);
        }
    }
}