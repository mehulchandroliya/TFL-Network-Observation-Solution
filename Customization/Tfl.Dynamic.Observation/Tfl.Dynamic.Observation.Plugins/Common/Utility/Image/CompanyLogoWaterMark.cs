using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.FileTypeAttachment;

namespace Tfl.Dynamic.Observation.Plugins.Common.Utility.Image
{
    public static class CompanyLogoWaterMark
    {
        public static byte[] Apply(byte[] image)
        {
            using (var ms = new MemoryStream(image))
            using (var bitmap = new Bitmap(ms))
            using (var g = Graphics.FromImage(bitmap))
            using (var pen = new Pen(ColorTranslator.FromHtml("#0019a7"), 6))
            using (var brush = new SolidBrush(ColorTranslator.FromHtml("#0019a8")))
            using (var outputMs = new MemoryStream())
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawEllipse(pen, bitmap.Width - 37, bitmap.Height - 41, 26, 26);
                g.FillRectangle(brush, bitmap.Width - 44, bitmap.Height - 31, 40, 6);
                bitmap.Save(outputMs, ImageFormat.Jpeg);
                return outputMs.ToArray();
            }
        }
    }
}
