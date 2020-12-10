using System.Collections.Generic;

namespace BwtCompression.Coders.Rle
{
    internal class RleCoder
    {
        private List<byte> _bytes;

        internal List<byte> Encode(List<byte> bytes)
        {
            _bytes = bytes;

            var encodedBytes = new List<byte>();
            var currentlyCounting = _bytes[0];
            byte count = 0;
            foreach (var element in _bytes)
            {
                if (currentlyCounting == element && count < 255)
                {
                    count++;
                }
                else
                {
                    encodedBytes.Add(currentlyCounting);
                    encodedBytes.Add(count);
                    currentlyCounting = element;
                    count = 1;
                }
            }
            encodedBytes.Add(currentlyCounting);
            encodedBytes.Add(count);

            return encodedBytes;
        }

        internal List<byte> Decode(List<byte> bytes)
        {
            _bytes = bytes;

            var decodedBytes = new List<byte>();
            for (int i = 0; i < _bytes.Count; i += 2)
            {
                var byteValue = _bytes[i];
                var byteCount = _bytes[i + 1];
                for (int j = 0; j < byteCount; j++)
                {
                    decodedBytes.Add(byteValue);
                }
            }

            return decodedBytes;
        }
    }
}
