using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Imaging.Extensions
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Converts bitmap to 2bit
        /// </summary>
        /// <param name="original"></param>
        /// <param name="disposeOriginal"></param>
        /// <returns>Bitmap.</returns>
        public static Bitmap ConvertTo2Bit(this Bitmap original, bool disposeOriginal = true)
        {
            // Create image for manipulating
            Bitmap edit = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            edit.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            using (Graphics g = Graphics.FromImage(edit))
            {
                g.DrawImageUnscaled(original, 0, 0);
                g.Dispose();
            }
            // Dispose original image if flag set            
            if (disposeOriginal)
            {
                original.Dispose();
            }

            // Copy image data to binary array
            BitmapData editData = edit.LockBits(new Rectangle(0, 0, edit.Width, edit.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int imageSize = (editData.Stride * editData.Height);
            byte[] editBuffer = new byte[imageSize];
            Marshal.Copy(editData.Scan0, editBuffer, 0, imageSize);
            edit.UnlockBits(editData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(edit.Width, edit.Height, PixelFormat.Format1bppIndexed);
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = (destinationData.Stride * destinationData.Height);
            byte[] destinationBuffer = new byte[imageSize];

            // Create variables for iterating and converting
            byte destinationValue = 0;
            int editIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            int pixelValue = 128;
            int height = edit.Height;
            int width = edit.Width;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                editIndex = y * editData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    pixelTotal = editBuffer[editIndex + 1];
                    pixelTotal += editBuffer[editIndex + 2];
                    pixelTotal += editBuffer[editIndex + 3];

                    if (pixelTotal > (int)550)
                    {
                        destinationValue += (byte)pixelValue;
                    }

                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex += 1;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    editIndex += 4;
                }

                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);
            destination.UnlockBits(destinationData);
            edit.Dispose();

            // Return converted bitmap
            return destination;
        }
    }
}
