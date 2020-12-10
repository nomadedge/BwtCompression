using BwtCompression.Coders.Bwt;
using BwtCompression.Coders.Mtf;
using BwtCompression.Coders.Rle;
using BwtCompression.Coders.Unary;
using System.Collections.Generic;

namespace BwtCompression.Coders
{
    public class Encoder
    {
        private readonly List<byte> _originalBytes;
        private List<byte> _encodedBytes;

        public Encoder(List<byte> originalBytes)
        {
            _originalBytes = originalBytes;
        }

        public List<byte> Encode()
        {
            var nextBytes = _originalBytes;
            var isRle = false;

            var rleCoder = new RleCoder();
            var rleBytes = rleCoder.Encode(_originalBytes);
            if (rleBytes.Count < _originalBytes.Count)
            {
                nextBytes = rleBytes;
                isRle = true;
            }

            var bwtCoder = new BwtCoder();
            var bwtModel = bwtCoder.Encode(nextBytes);

            var mtfCoder = new MtfCoder();
            var mtfBytes = mtfCoder.Encode(bwtModel.Bytes);

            var unaryCoder = new UnaryCoder();
            var unaryModel = unaryCoder.Encode(mtfBytes);

            var encodedModel = new EncodedModel(isRle, bwtModel.OriginalIndex, unaryModel.Frequencies, unaryModel.Bytes);
            _encodedBytes = encodedModel.ToByteList();

            return _encodedBytes;
        }
    }
}
