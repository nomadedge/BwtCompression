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
            var isRle = false;

            var nextBytes = _originalBytes;

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

            if (unaryModel.Bytes.Count < mtfBytes.Count)
            {
                var encodedModel = new EncodedModel(isRle, true, bwtModel.OriginalIndex, unaryModel.Frequencies, unaryModel.Bytes);
                _encodedBytes = encodedModel.ToByteList();
            }
            else
            {
                var encodedModel = new EncodedModel(isRle, false, bwtModel.OriginalIndex, null, mtfBytes);
                _encodedBytes = encodedModel.ToByteList();
            }

            return _encodedBytes;
        }
    }
}
