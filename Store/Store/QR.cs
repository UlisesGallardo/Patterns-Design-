using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.Json;
using MessagingToolkit.QRCode.Codec.Data;
using System.Drawing.Imaging;
using System.IO;

namespace Store
{
    public class QR : InformationDissemination
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// 
        private Bitmap qr = null;

        public Bitmap Create(string  data)
        {
            var encoder = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
            encoder.QRCodeScale = 6;
            qr = encoder.Encode(data);
            return qr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Read(string path) 
        {   
            Image img;
            using (var bmpTemp = new Bitmap(path))
            {
                img = new Bitmap(bmpTemp);
            }

            var decoder = new MessagingToolkit.QRCode.Codec.QRCodeDecoder();
            string data = decoder.Decode(new QRCodeBitmapImage(img as Bitmap));
            return data;
        }

        public void Save(string path)
        {
            if (qr != null)
            {
                Image img = qr;
                bool result = File.Exists(path);
                if (result == true)
                {
                    File.Delete(path); 
                }
                img.Save(path, ImageFormat.Png);
                img.Dispose();
            }
        }
    }
}
