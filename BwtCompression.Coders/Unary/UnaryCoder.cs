using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BwtCompression.Coders.Unary
{
    internal class UnaryCoder
    {
        private class Frequency
        {
            public byte Key { get; set; }
            public int Value { get; set; }
        }

        private List<byte> _bytes;

        internal UnaryModel Encode(List<byte> bytes)
        {
            _bytes = bytes;

            var byteLength = 256;

            byte initialValue = 0;
            var frequencies = Enumerable
                .Range(0, byteLength)
                .Select(i => new Frequency
                {
                    Key = initialValue++,
                    Value = 0
                })
                .ToList();
            foreach (var element in _bytes)
            {
                frequencies[element].Value++;
            }
            frequencies = frequencies.OrderByDescending(f => f.Value).ToList();

            var alphabet = new Dictionary<byte, BitArray>(byteLength);
            var currentWeight = 1;
            for (int i = 0; i < byteLength; i++)
            {
                var frequency = frequencies[i];
                if (frequency.Value != 0)
                {
                    var value = new BitArray(currentWeight, true);
                    value[value.Count - 1] = false;
                    alphabet[frequency.Key] = value;
                    currentWeight++;
                }
            }

            var lengthBytes = 0;
            foreach (var symbol in alphabet)
            {
                lengthBytes += frequencies.First(f => f.Key == symbol.Key).Value * symbol.Value.Count;
            }
            lengthBytes = (lengthBytes - 1) / 8 + 1;
            var lengthBits = lengthBytes * 8;

            var encodedBools = new bool[lengthBits];
            var currentPosition = 0;
            foreach (var element in _bytes)
            {
                alphabet[element].CopyTo(encodedBools, currentPosition);
                currentPosition += alphabet[element].Length;
            }
            for (int i = currentPosition; i < lengthBits; i++)
            {
                encodedBools[i] = true;
            }
            var encodedBits = new BitArray(encodedBools);

            var encodedBytes = new byte[lengthBytes];
            encodedBits.CopyTo(encodedBytes, 0);

            var frequenciesDictionary = new Dictionary<byte, int>();
            for (int i = 0; i < byteLength; i++)
            {
                if (frequencies[i].Value != 0)
                {
                    frequenciesDictionary[frequencies[i].Key] = frequencies[i].Value;
                }
            }

            var encodedBytesList = new List<byte>(encodedBytes);

            var model = new UnaryModel(frequenciesDictionary, encodedBytesList);
            return model;
        }

        internal List<byte> Decode(UnaryModel model)
        {
            var byteLength = 256;

            var alphabet = new Dictionary<int, byte>(byteLength);
            var currentWeight = 0;
            foreach (var frequency in model.Frequencies.OrderByDescending(f => f.Value))
            {
                alphabet[currentWeight] = frequency.Key;
                currentWeight++;
            }

            var encodedBits = new BitArray(model.Bytes.ToArray());
            var decodedBytes = new List<byte>();
            var oneCount = 0;
            for (int i = 0; i < encodedBits.Count; i++)
            {
                if (encodedBits[i])
                {
                    oneCount++;
                }
                else
                {
                    decodedBytes.Add(alphabet[oneCount]);
                    oneCount = 0;
                }
            }

            return decodedBytes;
        }
    }
}
