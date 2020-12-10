using System.Collections.Generic;

namespace BwtCompression.Coders.Unary
{
    internal class UnaryModel
    {
        internal Dictionary<byte, int> Frequencies { get; }
        internal List<byte> Bytes { get; }

        internal UnaryModel(Dictionary<byte, int> frequencies, List<byte> bytes)
        {
            Frequencies = frequencies;
            Bytes = bytes;
        }
    }
}
