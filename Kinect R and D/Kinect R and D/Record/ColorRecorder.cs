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
using System.Drawing;


namespace Kinect_R_and_D.Record
{
    public class ColorRecorder
    {

        private const int BUFFERSIZE = 5;
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap[] colorBitmap;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        public byte[] colorPixels;
        private int framesNum = 0;
        private KinectSensor kinect;
        public ColorRecorder(KinectSensor kinect)
        {
            this.kinect = kinect;
            // Allocate space to put the pixels we'll receive
            this.colorPixels = new byte[kinect.ColorStream.FramePixelDataLength];
            colorBitmap = new WriteableBitmap[BUFFERSIZE];
            // This is the bitmap we'll display on-screen.
            for (int i = 0; i < BUFFERSIZE; i++)
            this.colorBitmap[i] = new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

        }

        public void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    if(0 == (framesNum+1) % BUFFERSIZE) //if buffer is full - empty buffer to file\s
                    {
                        for (int i = framesNum-BUFFERSIZE+1; i < framesNum+1; i++)
                        {
                            CreateThumbnail(@"images\color"+i+".bmp", ConvertWriteableBitmapToBitmapImage(colorBitmap[i%BUFFERSIZE]));
                        }

                    }
                     
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap

                    this.colorBitmap[framesNum % BUFFERSIZE].WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap[framesNum % BUFFERSIZE].PixelWidth, this.colorBitmap[framesNum % BUFFERSIZE].PixelHeight),
                            this.colorPixels,
                            this.colorBitmap[framesNum % BUFFERSIZE].PixelWidth * sizeof(int),
                            0);
                        framesNum++;
                }
            }
        }

        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                    stream5.Close();
                }
            }
        }

        private BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        public WriteableBitmap ColorBitmap
        {
            get
            {
                return this.colorBitmap[framesNum%BUFFERSIZE];
            }

        }

    }
}