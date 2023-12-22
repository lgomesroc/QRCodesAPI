using System.Drawing;
using ZXing;
using ZXing.Common;

namespace QRCodesAPI
{
    public class BarcodeWriter
    {
        public BarcodeFormat Format { get; internal set; }
        public EncodingOptions Options { get; internal set; }

        internal Bitmap Write(string data)
        {
            throw new NotImplementedException();
        }
    }
}
