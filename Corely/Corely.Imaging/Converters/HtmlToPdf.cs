using Corely.Imaging.Core;
using Corely.Imaging.Core.Margins;
using Corely.Imaging.Core.Rectangles;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Imaging.Converters
{
    public class HtmlToPdf
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public HtmlToPdf() { }

        #endregion

        #region Properties

        /// <summary>
        /// PageType
        /// </summary>
        public PageType PageType { get; set; } = PageType.A4;

        /// <summary>
        /// Page orientation
        /// </summary>
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        /// <summary>
        /// Color model
        /// </summary>
        public ColorModel ColorModel { get; set; } = ColorModel.RGB;

        /// <summary>
        /// Page margin
        /// </summary>
        public PointMargin PageMargin { get; set; } = new PointMargin(20, 10);

        /// <summary>
        /// Custom page size
        /// </summary>
        public PointRectangle CustomPageSize { get; set; }

        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Creation Date
        /// </summary>
        public DateTime? CreationDate { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Convert HTML file to PDF file
        /// </summary>
        /// <param name="htmlFilePath"></param>
        /// <param name="pdfFilePath"></param>
        public void FileToPDFFile(string htmlFilePath, string pdfFilePath)
        {
            byte[] bytes = FileToPDF(htmlFilePath);
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Convert HTML stream to PDF file
        /// </summary>
        /// <param name="htmlStream"></param>
        /// <param name="pdfFilePath"></param>
        public void ToPDFFile(Stream htmlStream, string pdfFilePath)
        {
            byte[] bytes = ToPDF(htmlStream);
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Convert HTML to PDF file
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pdfFilePath"></param>
        public void ToPDFFile(string html, string pdfFilePath)
        {
            byte[] bytes = ToPDF(html);
            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Convert from HTML file path to PDF stream
        /// </summary>
        /// <param name="htmlFilePath"></param>
        /// <returns></returns>
        public Stream FileToPDFStream(string htmlFilePath)
        {
            string html = File.ReadAllText(htmlFilePath);
            byte[] bytes = ToPDF(html);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// Convert from HTML file path to PDF
        /// </summary>
        /// <param name="htmlFilePath"></param>
        /// <returns></returns>
        public byte[] FileToPDF(string htmlFilePath)
        {
            string html = File.ReadAllText(htmlFilePath);
            return ToPDF(html);
        }

        /// <summary>
        /// Convert HTML stream to PDF stream
        /// </summary>
        /// <param name="htmlStream"></param>
        /// <returns></returns>
        public Stream ToPDFStream(Stream htmlStream)
        {
            byte[] bytes = ToPDF(htmlStream);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// Convert HTML to PDF stream
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public Stream ToPDFStream(string html)
        {
            byte[] bytes = ToPDF(html);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// Convert HTML stream to PDF
        /// </summary>
        /// <param name="htmlStream"></param>
        /// <returns></returns>
        public byte[] ToPDF(Stream htmlStream)
        {
            string htmlText = "";
            using (StreamReader reader = new StreamReader(htmlStream))
            {
                htmlText = reader.ReadToEnd();
            }
            return ToPDF(htmlText);
        }

        /// <summary>
        /// Convert HTML to PDF
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public byte[] ToPDF(string html)
        {
            // Create converter and set options
            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = GetSelectPdfPageSize();
            converter.Options.PdfPageOrientation = GetSelectPdfPageOrientation();
            converter.Options.ColorSpace = GetSelectPdfColorSpace();
            if(PageMargin != null)
            {
                converter.Options.MarginTop = Convert.ToInt32(PageMargin.Top);
                converter.Options.MarginBottom = Convert.ToInt32(PageMargin.Bottom);
                converter.Options.MarginLeft = Convert.ToInt32(PageMargin.Left);
                converter.Options.MarginRight = Convert.ToInt32(PageMargin.Right);
            }
            if(CustomPageSize != null)
            {
                converter.Options.PdfPageCustomSize = new System.Drawing.SizeF((float)CustomPageSize.Width, (float)CustomPageSize.Height);
            }
            converter.Options.PdfDocumentInformation.Author = Author ?? "";
            converter.Options.PdfDocumentInformation.Title = Title ?? "";
            converter.Options.PdfDocumentInformation.Subject = Subject ?? "";
            converter.Options.PdfDocumentInformation.CreationDate = CreationDate ?? DateTime.Now;
            // Connvert and return PDF bytes
            PdfDocument doc = converter.ConvertHtmlString(html);
            byte[] pdfBytes = doc.Save();
            return pdfBytes;
        }

        /// <summary>
        /// Get SelectPDF color space
        /// </summary>
        /// <returns></returns>
        private PdfColorSpace GetSelectPdfColorSpace()
        {
            switch (ColorModel)
            {
                case ColorModel.CMYK: return PdfColorSpace.CMYK;
                case ColorModel.GreyScale: return PdfColorSpace.GrayScale;
                case ColorModel.RGB:
                default: return PdfColorSpace.RGB;
            }
        }

        /// <summary>
        /// Get SelectPDF page orientation
        /// </summary>
        /// <returns></returns>
        private PdfPageOrientation GetSelectPdfPageOrientation()
        {
            switch (PageOrientation)
            {
                case PageOrientation.Landscape: return PdfPageOrientation.Landscape;
                case PageOrientation.Portrait:
                default: return PdfPageOrientation.Portrait;
            }
        }

        /// <summary>
        /// Get SelectPDF page size
        /// </summary>
        /// <returns></returns>
        private PdfPageSize GetSelectPdfPageSize()
        {
            switch (PageType)
            {
                case PageType.RA0:
                case PageType.A0: return PdfPageSize.A0;
                case PageType.RA1:
                case PageType.A1: return PdfPageSize.A1;
                case PageType.RA2:
                case PageType.A2: return PdfPageSize.A2;
                case PageType.RA3:
                case PageType.A3: return PdfPageSize.A3;
                case PageType.RA4:
                case PageType.A4:return PdfPageSize.A4;
                case PageType.RA5:
                case PageType.A5: return PdfPageSize.A5;
                case PageType.B0: return PdfPageSize.B0;
                case PageType.B1: return PdfPageSize.B1;
                case PageType.B2: return PdfPageSize.B2;
                case PageType.B3: return PdfPageSize.B3;
                case PageType.B4: return PdfPageSize.B4;
                case PageType.B5: return PdfPageSize.B5;
                case PageType.Ledger: return PdfPageSize.Ledger;
                case PageType.Legal: return PdfPageSize.Legal;
                case PageType.Letter: return PdfPageSize.Letter;
                case PageType.Crown:
                case PageType.Demy:
                case PageType.DoubleDemy:
                case PageType.Elephant:
                case PageType.Executive:
                case PageType.Folio:
                case PageType.Foolscap:
                case PageType.GovernmentLetter:
                case PageType.LargePost:
                case PageType.Medium:
                case PageType.Post:
                case PageType.QuadDemy:
                case PageType.Quarto:
                case PageType.Royal:
                case PageType.Size10x14:
                case PageType.Statement:
                case PageType.STMT:
                case PageType.Tabloid:
                case PageType.Undefined:
                default: return PdfPageSize.Custom;
            }
        }

        #endregion
    }
}
