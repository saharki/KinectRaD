using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Timers;
using Kinect_R_and_D.Record;

namespace Kinect_R_and_D.Record
{
    class SkeletonPositionTracking
    {
        private const string filePath = @"JointPosition.csv";
        private string csvHeadline = "Head X, Head Y, Head Z, HandLeft X, HandLeft Y, HandLeft Z, WristLeft X, WristLeft Y," +
 "WristLeft Z, HandRight X, HandRight Y, HandRight Z, WristRight X, WristRight Y, WristRight Z" + System.Environment.NewLine;
        private Skeleton[] skeletonData;
        private string CSVFileRow;
        private int CSVFileRowCount; //To optimize the write operation. 

        public SkeletonPositionTracking (int frameSkeletonArrayLength)
        {
            skeletonData = new Skeleton[frameSkeletonArrayLength];
            File.WriteAllText(filePath, csvHeadline);
        }
        public void kinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame.
            {
                if (skeletonFrame != null && skeletonData != null) // check that a frame is available.
                {
                    skeletonFrame.CopySkeletonDataTo(skeletonData); // get the skeletal information in this frame.

                    CSVFileRow += skeletonData[0].Joints[JointType.Head].Position.X.ToString() +
                          "," + skeletonData[0].Joints[JointType.Head].Position.Y.ToString() +
                            "," + skeletonData[0].Joints[JointType.Head].Position.Z.ToString();

                    CSVFileRow += "," + skeletonData[0].Joints[JointType.HandLeft].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandLeft].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandLeft].Position.Z.ToString();

                    CSVFileRow += "," + skeletonData[0].Joints[JointType.WristLeft].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristLeft].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristLeft].Position.Z.ToString();

                    CSVFileRow += "," + skeletonData[0].Joints[JointType.HandRight].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandRight].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandRight].Position.Z.ToString();

                    CSVFileRow += "," + skeletonData[0].Joints[JointType.WristRight].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristRight].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristRight].Position.Z.ToString() + System.Environment.NewLine;
                    //Write to the file every 5 frames, for optimization.
                    if (++CSVFileRowCount == 5)
                    {
                        File.AppendAllText(filePath, CSVFileRow);
                        CSVFileRowCount = 0;
                        CSVFileRow = "";
                    }


                    // Console.Write("|X7: " + skeletonData[0].Joints[JointType.KneeLeft].Position.X.ToString() +
                    //" Y7: " + skeletonData[0].Joints[JointType.KneeLeft].Position.Y.ToString() +
                    //" Z7: " + skeletonData[0].Joints[JointType.KneeLeft].Position.Z.ToString());

                    // Console.Write("|X8: " + skeletonData[0].Joints[JointType.FootLeft].Position.X.ToString() +
                    //" Y8: " + skeletonData[0].Joints[JointType.FootLeft].Position.Y.ToString() +
                    //" Z8: " + skeletonData[0].Joints[JointType.FootLeft].Position.Z.ToString());

                    // Console.Write("|X9: " + skeletonData[0].Joints[JointType.KneeRight].Position.X.ToString() +
                    //  " Y9: " + skeletonData[0].Joints[JointType.KneeRight].Position.Y.ToString() +
                    //" Z9: " + skeletonData[0].Joints[JointType.KneeRight].Position.Z.ToString());

                    // Console.WriteLine("|X10: " + skeletonData[0].Joints[JointType.FootRight].Position.X.ToString() +
                    //" Y10: " + skeletonData[0].Joints[JointType.FootRight].Position.Y.ToString() +
                    //" Z10: " + skeletonData[0].Joints[JointType.FootRight].Position.Z.ToString());
                }
            }
        }
    }
}
