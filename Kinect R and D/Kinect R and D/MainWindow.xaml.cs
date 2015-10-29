using System;
using System.IO;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Timers;


namespace Kinect_R_and_D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensor kinect;
        private Skeleton[] skeletonData;
        private string filePath = @"Step0.csv";
        private string CSVFileRow;
        private int CSVFileRowCount; //To optimize the write operation. 


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            //SetTimer();
            Utility.SetTimer(15000);
        }



        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //Initialize sensor UI helper called sensorChooser and start the sensor.
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
            this.kinect = this.sensorChooser.Kinect;
            // Allocate and start the skeleton Data stream.
            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];
            this.kinect.SkeletonStream.Enable();
            // Get Ready for Skeleton Ready Events.
            this.kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReady);
            this.kinect.Start(); // Start Kinect sensor
            //Create the file for CSV values of the Joints.
            //ToDo: add the kinectRegion on load.
            //if (!error)
            //    kinectRegion.KinectSensor = e.NewSensor;
            string headline = "Head X, Head Y, Head Z, HandLeft X, HandLeft Y, HandLeft Z, WristLeft X, WristLeft Y," +
              "WristLeft Z, HandRight X, HandRight Y, HandRight Z, WristRight X, WristRight Y, WristRight Z" + System.Environment.NewLine;
            File.WriteAllText(filePath, headline);
        }

        private void kinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame.
            {
                if (skeletonFrame != null && this.skeletonData != null) // check that a frame is available.
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData); // get the skeletal information in this frame.

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

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs e)
        {
            // MessageBox.Show(e.NewSensor == null ? "No Kinect" : e.NewSensor.Status.ToString());

            bool error = false;
            if (e.OldSensor != null)
            {
                try
                {
                    e.OldSensor.DepthStream.Range = DepthRange.Default;
                    e.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    e.OldSensor.DepthStream.Disable();
                    e.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (e.NewSensor != null)
            {
                try
                {
                    e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    e.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        e.NewSensor.DepthStream.Range = DepthRange.Default;
                        e.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        e.NewSensor.DepthStream.Range = DepthRange.Default;
                        e.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        error = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    error = true;
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
            if (!error)
                kinectRegion.KinectSensor = e.NewSensor;
        }

    }
    /// <summary>
    /// Utilities for the project.
    /// </summary>
    public class Utility
    {
        private static Timer aTimer;

        public static void SetTimer(int time)
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}