using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using ZXing.QrCode;

namespace QRCodesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class QRCodeController : ControllerBase
    {
        [HttpGet("generate")]
        public IActionResult GenerateQRCode([FromQuery] string data)
        {
            try
            {

                BarcodeWriter barcodeWriter = new BarcodeWriter();
                barcodeWriter.Format = ZXing.BarcodeFormat.QR_CODE;

                ZXing.Common.EncodingOptions options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
                };
                barcodeWriter.Options = options;

                Bitmap qrCodeBitmap = barcodeWriter.Write(data);

                MemoryStream stream = new MemoryStream();
                qrCodeBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                return File(stream.ToArray(), "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao gerar o QR code: {ex.Message}");
            }
        }

        [HttpPost("read")]
        public IActionResult ReadQRCode([FromBody] QRCodeRequest request)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(request.Base64Image);

                MemoryStream stream = new MemoryStream(imageBytes);

                Bitmap qrCodeBitmap = new Bitmap(stream);

                BarcodeReader barcodeReader = new BarcodeReader();

                var result = barcodeReader.Decode(qrCodeBitmap);

                if (result != null)
                {
                    return Ok(new DataText(result.Text));
                }
                 
                return BadRequest("Nenhum QR code encontrado na imagem.");
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao ler o QR code: {ex.Message}");
            }
        }
    }

    public class QRCodeRequest
    {
        public string Base64Image { get; set; }
    }

    internal class DataText
    {
        public object Data { get; }
        public string Text { get; }

        public DataText(object data)
        {
            Data = data;
        }

        public DataText(string text, Result result)
        {
            Text = text;
            Text = result.Text;
        }

        public override bool Equals(object obj)
        {
            return obj is DataText other &&
                   EqualityComparer<object>.Default.Equals(Data, other.Data) && 
                   EqualityComparer<string>.Default.Equals(Text, other.Text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Text);
        }
    }
}
