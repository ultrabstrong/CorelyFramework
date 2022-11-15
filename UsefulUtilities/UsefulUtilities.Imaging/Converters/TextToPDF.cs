using UsefulUtilities.Imaging.Core;
using UsefulUtilities.Imaging.Core.Margins;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UsefulUtilities.Imaging.Converters
{
    [Serializable]
    public class TextToPDF
    {
        #region Constructors

        public TextToPDF() { }

        #endregion

        #region Properties

        /// <summary>
        /// Margin for page
        /// </summary>
        public Margin Margin { get; set; } = new PointMargin(36);

        /// <summary>
        /// Font name
        /// </summary>
        public string FontName { get; set; } = "Consolas";

        /// <summary>
        /// Font size
        /// </summary>
        public int FontSize { get; set; } = 9;

        /// <summary>
        /// Allowed page types
        /// </summary>
        public PageType[] AllowedPageTypes { get; set; } = new[] { PageType.Letter };

        #endregion

        #region AutoFit Methods

        /// <summary>
        /// Autofit text file to pdf file
        /// </summary>
        /// <param name="filepath"></param>
        public void AutoFitFileToPDFFile(string filepath, string outpath)
        {
            using (PdfDocument pdf = AutoFitFileToPDF(filepath))
            {
                pdf.Save(outpath);
            }
        }

        /// <summary>
        /// Autofit text file to pdf stream
        /// </summary>
        /// <param name="filepath"></param>
        public Stream AutoFitFileToPDFStream(string filepath)
        {
            Stream stream = new MemoryStream();
            using (PdfDocument pdf = AutoFitFileToPDF(filepath))
            {
                pdf.Save(stream);
            }
            return stream;
        }

        /// <summary>
        /// Autofit text file to pdf byte array
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] AutoFitFileToPDFBytes(string filepath)
        {
            byte[] bytes = null;
            using (Stream stream = AutoFitFileToPDFStream(filepath))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Autofit text to pdf file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="outpath"></param>
        public void AutoFitTextToPDFFile(string text, string outpath)
        {
            using (PdfDocument pdf = AutoFitTextToPDF(text))
            {
                pdf.Save(outpath);
            }
        }

        /// <summary>
        /// Autofit text to pdf stream
        /// </summary>
        /// <param name="text"></param>
        /// <param name="outpath"></param>
        public Stream AutoFitTextToPDFStream(string text)
        {
            Stream stream = new MemoryStream();
            using (PdfDocument pdf = AutoFitTextToPDF(text))
            {
                pdf.Save(stream);
            }
            return stream;
        }

        /// <summary>
        /// Autofit text to pdf byte array
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] AutoFitTextToPDFBytes(string text)
        {
            byte[] bytes = null;
            using (Stream stream = AutoFitTextToPDFStream(text))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Auto fit file to PDF
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private PdfDocument AutoFitFileToPDF(string filepath)
        {
            if (!File.Exists(filepath)) { throw new FileNotFoundException(filepath); }
            string[] lines = File.ReadAllLines(filepath);
            return AutoFitLinesToPDF(lines);
        }

        /// <summary>
        /// Auto fit text and return pdf
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private PdfDocument AutoFitTextToPDF(string text)
        {
            string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return AutoFitLinesToPDF(lines);
        }

        /// <summary>
        /// Autofit lines to pdf document
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private PdfDocument AutoFitLinesToPDF(string[] lines)
        {
            // Create variables for adding pages
            int curline = 0;
            PdfDocument pdf = new PdfDocument();
            XFont font = new XFont(FontName, FontSize);
            do
            {
                // Initialize current list of lines with the current line
                List<string> curlines = new List<string>();
                // Create page and settings
                PdfPage pdfPage = pdf.AddPage();
                pdfPage.Size = (PdfSharp.PageSize)(int)AllowedPageTypes[0];
                XGraphics graph = XGraphics.FromPdfPage(pdfPage, XGraphicsPdfPageOptions.Prepend);
                XSize size = new XSize();
                XSize sizeh = new XSize(0, 0);
            addlines:
                // Append line to current lines list
                if (curline < lines.Length)
                {
                    // Add new line of text
                    curlines.Add(lines[curline++].ToString());
                    // Adjust size for new line of text
                    sizeh = MeasureTotalHeightAndMaxWidth(sizeh, graph, curlines[curlines.Count - 1], font);
                    // Add lines until height of lines exceeds page height
                    if (sizeh.Height < pdfPage.Height - Margin.TAndB)
                    {
                        // Record current size
                        size = sizeh;
                        // Add next line
                        goto addlines;
                    }
                    else
                    {
                        // Remove last line since it exceeds height
                        curline--;
                        curlines.RemoveAt(curlines.Count - 1);
                    }
                }
                // Fit page size to width until text fits or allowed types run out
                bool sizechanged = false;
                int nextpagetype = AllowedPageTypes.ToList().IndexOf((PageType)(int)pdfPage.Size) + 1;
                while (size.Width > pdfPage.Width - Margin.LAndR && nextpagetype < AllowedPageTypes.Length)
                {
                    sizechanged = true;
                    pdfPage.Size = (PdfSharp.PageSize)(int)AllowedPageTypes[nextpagetype];
                    nextpagetype++;
                }
                // Add more lines if width changed
                if (sizechanged) { goto addlines; }
                // Draw text on page once linces are selected
                XTextFormatter tf = new XTextFormatter(graph);
                XRect rectangle = new XRect(Margin.Left, Margin.Top, pdfPage.Width.Point - Margin.LAndR, pdfPage.Height.Point - Margin.TAndB);
                string text = string.Join(Environment.NewLine, curlines);
                tf.DrawString(text, font, XBrushes.Black, rectangle, XStringFormats.TopLeft);
                // Go to next page if there are more lines
            }
            while (curline < lines.Length);
            // Return pdf document
            return pdf;
        }

        /// <summary>
        /// Measure size of single line of text
        /// </summary>
        /// <param name="totalsize"></param>
        /// <param name="graph"></param>
        /// <param name="line"></param>
        /// <param name="font"></param>
        private XSize MeasureTotalHeightAndMaxWidth(XSize totalsize, XGraphics graph, string line, XFont font)
        {
            XSize linesize = graph.MeasureString(line, font);
            totalsize.Height += linesize.Height;
            if (linesize.Width > totalsize.Width)
            {
                totalsize.Width = linesize.Width;
            }
            return totalsize;
        }

        /// <summary>
        /// Measure size of all lines of text
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private XSize MeasureTotalHeightAndMaxWidth(XGraphics graph, List<string> lines, XFont font)
        {
            double totalHeight = 0;
            double maxWidth = 0;
            XSize size = new XSize(0, 0);
            foreach (string line in lines)
            {
                MeasureTotalHeightAndMaxWidth(size, graph, line, font);
            }

            return new XSize(maxWidth, totalHeight);
        }

        #endregion
    }
}
