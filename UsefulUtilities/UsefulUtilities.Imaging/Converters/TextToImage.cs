using UsefulUtilities.Imaging.Core;
using UsefulUtilities.Imaging.Core.Margins;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace UsefulUtilities.Imaging.Converters
{
    public class TextToImage
    {
        #region Constructors

        public TextToImage() { }

        #endregion

        #region Properties

        /// <summary>
        /// Margin for page
        /// </summary>
        public Margin Margin { get; set; } = new PixelMargin(150);

        /// <summary>
        /// Font name
        /// </summary>
        public string FontName { get; set; } = "Consolas";

        /// <summary>
        /// Font size
        /// </summary>
        public int FontSize { get; set; } = 9;

        /// <summary>
        /// Dots per inch to use
        /// </summary>
        public float DPI { get; set; } = 300;

        /// <summary>
        /// Image format
        /// </summary>
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;

        #endregion

        #region Methods

        /// <summary>
        /// Write text file to bitmap file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="outpath"></param>
        public void WriteFileToBitmapFile(string filepath, string outpath)
        {
            using (Bitmap bmp = WriteFileToBitmap(filepath))
            {
                bmp.Save(outpath);
            }
        }

        /// <summary>
        /// Write text file to bitmap stream
        /// </summary>
        /// <param name="filepath"></param>
        public Stream WriteFileToBitmapStream(string filepath)
        {
            Stream stream = new MemoryStream();
            using (Bitmap bmp = WriteFileToBitmap(filepath))
            {
                bmp.Save(stream, ImageFormat);
            }
            return stream;
        }

        /// <summary>
        /// Write text file to bitmap byte array
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public byte[] WriteFileToBitmapBytes(string filepath)
        {
            byte[] bytes = null;
            using (Stream stream = WriteFileToBitmapStream(filepath))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Write text to bitmap file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="outpath"></param>
        public void WriteTextToBitmapFile(string text, string outpath)
        {
            using (Bitmap bmp = WriteTextToBitmap(text))
            {
                bmp.Save(outpath);
            }
        }

        /// <summary>
        /// Write text to bitmap stream
        /// </summary>
        /// <param name="filepath"></param>
        public Stream WriteTextToBitmapStream(string text)
        {
            Stream stream = new MemoryStream();
            using (Bitmap bmp = WriteTextToBitmap(text))
            {
                bmp.Save(stream, ImageFormat);
            }
            return stream;
        }

        /// <summary>
        /// Write text to bitmap byte array
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public byte[] WriteTextToBitmapBytes(string text)
        {
            byte[] bytes = null;
            using (Stream stream = WriteTextToBitmapStream(text))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Write text file to bitmap
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public Bitmap WriteFileToBitmap(string filepath)
        {
            if (!File.Exists(filepath)) { throw new FileNotFoundException(filepath); }
            string text = File.ReadAllText(filepath);
            return WriteTextToBitmap(text);
        }

        /// <summary>
        /// Write text to bitmap
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Bitmap WriteTextToBitmap(string text)
        {
            // Create font and string format
            Font font = new Font(FontName, FontSize);
            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.NoWrap & StringFormatFlags.NoClip & StringFormatFlags.NoFontFallback
            };
            // Get the size of the text
            SizeF textsize;
            using (Bitmap sizingbitmap = new Bitmap(1, 1))
            {
                using (Graphics graphics = Graphics.FromImage(sizingbitmap))
                {
                    textsize = graphics.MeasureString(text, font);
                }
            }
            // Create bitmap the size of the string
            int height = Convert.ToInt32(textsize.Height) + Convert.ToInt32(Margin.TAndB);
            int width = Convert.ToInt32(textsize.Width) + Convert.ToInt32(Margin.LAndR);
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Set graphics modes for highest resolution drawing
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                // Draw text on rectangle
                graphics.FillRectangle(Brushes.White, Convert.ToInt32(Margin.Left), Convert.ToInt32(Margin.Top), width - Convert.ToInt32(Margin.LAndR), height - Convert.ToInt32(Margin.TAndB));
                graphics.DrawString(text, font, Brushes.Black, 0, 0);
                // Flush graphics to bitmap
                graphics.Flush();
            }
            bitmap.SetResolution(DPI, DPI);
            // Return bitmap
            return bitmap;
        }

        #endregion
    }
}
