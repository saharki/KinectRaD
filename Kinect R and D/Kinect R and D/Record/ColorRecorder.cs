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
    class ColorRecorder
    {
        DateTime referenceTime;
        private WriteableBitmap colorBitmap;
        private byte[] colorPixels;
        internal ColorRecorder(WriteableBitmap writer)
        {
           // this.writer = writer;
            referenceTime = DateTime.Now;
        }
        public void RecordToBitmap(ColorImageFrame colorFrame, KinectSensor kinect)
        {
            if (colorFrame != null)
            {
                colorFrame.CopyPixelDataTo(this.colorPixels);
                this.colorBitmap = new WriteableBitmap(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                this.colorBitmap.WritePixels(
                new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                  this.colorPixels,
                this.colorBitmap.PixelWidth * sizeof(int), 0);
            }
        }
       /* public void Record(ColorImageFrame frame)
        {
            // Header
            writer.WritePixels()
            writer.Write((int)KinectRecordOptions.Color);

            // Data
            TimeSpan timeSpan = DateTime.Now.Subtract(referenceTime);
            referenceTime = DateTime.Now;
            writer.Write((long)timeSpan.TotalMilliseconds);
            writer.Write(frame.BytesPerPixel);
            writer.Write((int)frame.Format);
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            writer.Write(frame.FrameNumber);

            // Bytes
            writer.Write(frame.PixelDataLength);
            byte[] bytes = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(bytes);
            writer.Write(bytes);
        }*/
    }
}
