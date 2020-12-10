using BwtCompression.Coders.Bwt;
using BwtCompression.Coders.Mtf;
using BwtCompression.Coders.Rle;
using BwtCompression.Coders.Unary;
using System.Collections.Generic;

namespace BwtCompression.Coders
{
    public class Decoder
    {
        private readonly List<byte> _encodedBytes;
        private List<byte> _decodedBytes;

        public Decoder(List<byte> encodedBytes)
        {
            _encodedBytes = encodedBytes;
        }

        public List<byte> Decode()
        {
            var encodedModel = new EncodedModel(_encodedBytes);

            var mtfBytes = encodedModel.Bytes;

            if (encodedModel.IsUnary)
            {
                var unaryModel = new UnaryModel(encodedModel.Frequencies, encodedModel.Bytes);
                var unaryCoder = new UnaryCoder();
                mtfBytes = unaryCoder.Decode(unaryModel);
            }
            

            var mtfCoder = new MtfCoder();
            var bwtBytes = mtfCoder.Decode(mtfBytes);

            var bwtModel = new BwtModel(encodedModel.OriginalIndex, bwtBytes);
            var bwtCoder = new BwtCoder();
            var rleBytes = bwtCoder.Decode(bwtModel);

            if (encodedModel.IsRle)
            {
                var rleCoder = new RleCoder();
                _decodedBytes = rleCoder.Decode(rleBytes);
            }
            else
            {
                _decodedBytes = rleBytes;
            }
            

            return _decodedBytes;
        }
    }
}
