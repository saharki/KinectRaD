using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Kinect_R_and_D.Record
{
    public class ColorRecorder
    {
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;



        public void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        public byte[] ColorPixels
        {
            get
            {
                return this.colorPixels;
            }
            set
            {
                this.colorPixels = value;
            }
        }

        public WriteableBitmap ColorBitmap
        {
            get
            {
                return this.colorBitmap;
            }
            set
            {
                this.colorBitmap = value;
            }
        }
    }
}