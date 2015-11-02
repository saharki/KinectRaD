using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Kinect_R_and_D.Record
{
    public class ColorRecorder
    {

        private const int BUFFERSIZE = 5;
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        public WriteableBitmap colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        public byte[][] colorPixels;
        private int framesNum = 0;

        public ColorRecorder(KinectSensor kinect)
        {
            colorPixels = new byte[BUFFERSIZE][];
            // Allocate space to put the pixels we'll receive
            for (int i = 0; i < BUFFERSIZE; i++)
            this.colorPixels[i] = new byte[kinect.ColorStream.FramePixelDataLength];
            // This is the bitmap we'll display on-screen
            this.colorBitmap = new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

        }

        public void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    framesNum++;

                    if(0 == framesNum % BUFFERSIZE) //if buffer is full - empty buffer to file\s
                    {
                      /*  for (int i = 0; i < BUFFERSIZE; i++)
                        {
                            using (var ms = new MemoryStream(colorPixels[i]))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                                image.Save(@"images\image" + i + ".jpg");
                            }
                        }*/
                        framesNum=1;
                    }
                     
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels[framesNum]);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels[framesNum],
                        this.colorBitmap.PixelWidth * sizeof(int),
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

    }
}