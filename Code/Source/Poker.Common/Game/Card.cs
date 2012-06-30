using System;
using System.IO;

namespace Poker.Common.Game
{
    public class Card : IEquatable<Card>,IComparable<Card>
    {
        private CardSuiteType suite;
        private CardValueType value;

        public void Read(BinaryReader reader)
        {
            suite = (CardSuiteType) reader.ReadByte();
            value = (CardValueType) reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)suite);
            writer.Write((byte)value);
        }

        public override int GetHashCode()
        {
            return (int) ((int) suite*(int) value);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", value, suite);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Card);
        }

        public bool Equals(Card other)
        {
            if ( other == null )
            {
                return false;
            }

            return suite == other.suite && value == other.value;
        }

        public int CompareTo(Card other)
        {
            return value.CompareTo(other.value);
        }

        public Card()
        {}

        public Card(CardSuiteType suite,CardValueType value)
        {
            this.suite = suite;
            this.value = value;
        }
    }
}