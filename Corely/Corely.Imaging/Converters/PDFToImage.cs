using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corely.Imaging.Extensions;
using System.Drawing.Imaging;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;

namespace Corely.Imaging.Converters
{
    [Serializable]
    public class PDFToImage
    {
        #region Constructors

        public PDFToImage() { }

        #endregion

        #region Properties

        /// <summary>
        /// Convert to black and white
        /// </summary>
        public bool BlackAndWhite { get; set; } = false;

        /// <summary>
        /// Result DPI of image
        /// </summary>
        public int ResultDPI { get; set; } = 300;

        #endregion

        #region Tiff Methods

        /// <summary>
        /// Rasterize PDF path to tiff path
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <param name="tiffFilePath"></param>
        public void PDFToTiffFile(string pdfFilePath, string tiffFilePath)
        {
            byte[] bytes = PDFToTiffBytes(pdfFilePath);
            using (FileStream fs = new FileStream(tiffFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Rasterize PDF stream to tiff path
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <param name="tiffFilePath"></param>
        public void PDFToTiffFile(Stream pdfStream, string tiffFilePath)
        {
            byte[] bytes = PDFToTiffBytes(pdfStream);
            using (FileStream fs = new FileStream(tiffFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Rasterize PDF path to tiff stream
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        public Stream PDFToTiffStream(string pdfFilePath)
        {
            byte[] bytes = PDFToTiffBytes(pdfFilePath);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// Rasterize PDF stream to tiff stream
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        public Stream PDFToTiffStream(Stream pdfStream)
        {
            byte[] bytes = PDFToTiffBytes(pdfStream);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// Rasterize PDF path to tiff bytes
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        public byte[] PDFToTiffBytes(string pdfFilePath)
        {
            byte[] bytes = null;
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                bytes = PDFToTiffBytes(fs);
            }
            return bytes;
        }

        /// <summary>
        /// Rasterize PDF stream to tiff bytes
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public byte[] PDFToTiffBytes(Stream pdfStream)
        {
            // Convert PDF pages to image bytes
            List<byte[]> pageBytes = PDFToPageImageBytes(pdfStream, ImageFormat.Tiff);
            // Assmeble into multipage tiff
            byte[] tiffDocBytes = null;
            Image tiffDocImg = null;
            try
            {
                // Initialize variables for building document in memory
                using (MemoryStream tiffDocStream = new MemoryStream())
                using (EncoderParameters encoderParams = new EncoderParameters(2))
                {
                    // Iterate page bytes and load into memory stream
                    for (int i = 0; i < pageBytes.Count; i++)
                    {
                        using (MemoryStream pageStream = new MemoryStream(pageBytes[i]))
                        {
                            if (i == 0)
                            {
                                // Initialize document image with first page
                                tiffDocImg = Image.FromStream(pageStream);
                                // Set encoder parameters for first page
                                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionNone);
                                encoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                                // Save to document stream
                                tiffDocImg.Save(tiffDocStream, ImageCodecInfo.GetImageEncoders().First(m => m.MimeType == "image/tiff"), encoderParams);
                            }
                            else
                            {
                                // Append subsequent pages
                                using (Image pageBmp = Image.FromStream(pageStream))
                                {
                                    tiffDocImg.SaveAdd(pageBmp, UpdateParams(encoderParams));
                                }
                            }
                        }
                    }
                    // Flush the file
                    tiffDocBytes = tiffDocStream.ToArray();
                }
            }
            finally
            {
                // Dispose of final image 
                if(tiffDocImg != null)
                {
                    try { tiffDocImg.Dispose(); } catch { }
                }
            }
            return tiffDocBytes;
        }

        /// <summary>
        /// Update encoder parameters for subsequent pages
        /// </summary>
        /// <param name="ep"></param>
        /// <returns></returns>
        private EncoderParameters UpdateParams(EncoderParameters ep)
        {
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionNone);
            ep.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
            return ep;
        }

        #endregion

        #region Image Methods

        /// <summary>
        /// Rasterize PDF path to tiff bytes
        /// </summary>
        /// <param name="pdfFilePath"></param>
        /// <returns></returns>
        public List<byte[]> PDFToPageImageBytes(string pdfFilePath, ImageFormat format)
        {
            List<byte[]> bytes = null;
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                bytes = PDFToPageImageBytes(fs, format);
            }
            return bytes;
        }

        /// <summary>
        /// Rasterize PDF stream to page image bytes
        /// </summary>
        /// <param name="pdfStream"></param>
        /// <returns></returns>
        public List<byte[]> PDFToPageImageBytes(Stream pdfStream, ImageFormat format)
        {
            // Load dll from local file
            List<byte[]> bytes = new List<byte[]>();
            GhostscriptVersionInfo gvi = new GhostscriptVersionInfo($@"{Environment.CurrentDirectory}\gsdll32.dll");
            using (GhostscriptRasterizer rasta = new GhostscriptRasterizer())
            {
                // Open file and read pdf pages to tiff pages
                rasta.Open(pdfStream, gvi, true);
                for (int pageNum = 1; pageNum <= rasta.PageCount; pageNum++)
                {
                    using (MemoryStream pageStream = new MemoryStream())
                    using (Image pageImg = rasta.GetPage(ResultDPI, pageNum))
                    {
                        if (BlackAndWhite)
                        {
                            using (Bitmap bandw = ((Bitmap)pageImg).ConvertTo2Bit(true))
                            {
                                bandw.Save(pageStream, format);
                            }
                        }
                        else
                        {
                            pageImg.Save(pageStream, format);
                        }
                        bytes.Add(pageStream.ToArray());
                    }
                }
            }
            return bytes;
        }

        #endregion

    }
}
