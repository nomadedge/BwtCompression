using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BwtCompression.Coders
{
    [Serializable]
    internal class EncodedModel
    {
        internal bool IsRle { get; }
        internal int OriginalIndex { get; }
        internal Dictionary<byte, int> Frequencies { get; }
        internal List<byte> Bytes { get; }

        internal EncodedModel(
            bool isRle,
            int originalIndex,
            Dictionary<byte, int> frequencies,
            List<byte> bytes)
        {
            IsRle = isRle;
            OriginalIndex = originalIndex;
            Frequencies = frequencies;
            Bytes = bytes;
        }

        internal EncodedModel(List<byte> bytes)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(bytes.ToArray(), 0, bytes.Count);
                memoryStream.Position = 0;

                var encodedModel = binaryFormatter.Deserialize(memoryStream) as EncodedModel;

                IsRle = encodedModel.IsRle;
                OriginalIndex = encodedModel.OriginalIndex;
                Frequencies = encodedModel.Frequencies;
                Bytes = encodedModel.Bytes;
            }
        }

        internal List<byte> ToByteList()
        {
            var encodedBytes = new List<byte>();

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, this);
                encodedBytes = memoryStream.ToArray().ToList();
            }

            return encodedBytes;
        }
    }
}
