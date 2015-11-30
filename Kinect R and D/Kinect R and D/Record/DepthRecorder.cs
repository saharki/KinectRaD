using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Kinect_R_and_D.Record
{
    public class DepthRecoder
    {
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the depth data received from the camera
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixels;



        public void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.DepthPixels);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    // Convert the depth to RGB
                    int colorPixelIndex = 0;
                    for (int i = 0; i < this.DepthPixels.Length; ++i)
                    {
                        // Get the depth for this pixel
                        short depth = DepthPixels[i].Depth;

                        // To convert to a byte, we're discarding the most-significant
                        // rather than least-significant bits.
                        // We're preserving detail, although the intensity will "wrap."
                        // Values outside the reliable depth range are mapped to 0 (black).

                        // Note: Using conditionals in this loop could degrade performance.
                        // Consider using a lookup table instead when writing production code.
                        // See the KinectDepthViewer class used by the KinectExplorer sample
                        // for a lookup table example.
                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                        // Write out blue byte
                        this.ColorPixels[colorPixelIndex++] = intensity;

                        // Write out green byte
                        this.ColorPixels[colorPixelIndex++] = intensity;

                        // Write out red byte                        
                        this.ColorPixels[colorPixelIndex++] = intensity;

                        // We're outputting BGR, the last byte in the 32 bits is unused so skip it
                        // If we were outputting BGRA, we would write alpha here.
                        ++colorPixelIndex;
                    }

                    // Write the pixel data into our bitmap
                    this.ColorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.ColorBitmap.PixelWidth, this.ColorBitmap.PixelHeight),
                        this.ColorPixels,
                        this.ColorBitmap.PixelWidth * sizeof(int),
                        0);
                }
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

        public DepthImagePixel[] DepthPixels
        {
            get
            {
                return this.depthPixels;
            }

            set
            {
                this.depthPixels = value;
            }
        }
    }
}
