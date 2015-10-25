
namespace Step_0
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensor kinect;
        private Skeleton[] skeletonData;
        private string filePath = @"Step0.csv";
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            //this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            //this.sensorChooser.Start();

            this.sensorChooser.Start();
            kinect = sensorChooser.Kinect;

            kinect.SkeletonStream.Enable();
            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data


            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSkeletonFrameReady); // Get Ready for Skeleton Ready Events
            kinect.Start(); // Start Kinect sensor
            string headline = " head X, head y, head z, hand left x, hand left y, hand left z, wrist left x, wrist left y , wrist left z" + System.Environment.NewLine;

            File.WriteAllText(filePath, headline);
        }

        private void kinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && this.skeletonData != null) // check that a frame is available
                {

                    string delimiter = ",";


                    string str = "";



                    skeletonFrame.CopySkeletonDataTo(this.skeletonData); // get the skeletal information in this frame
                    str += skeletonData[0].Joints[JointType.Head].Position.X.ToString() +
      "," + skeletonData[0].Joints[JointType.Head].Position.Y.ToString() +
      "," + skeletonData[0].Joints[JointType.Head].Position.Z.ToString();

                    str += "," + skeletonData[0].Joints[JointType.HandLeft].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandLeft].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.HandLeft].Position.Z.ToString();

                    str += "," + skeletonData[0].Joints[JointType.WristLeft].Position.X.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristLeft].Position.Y.ToString() +
                         "," + skeletonData[0].Joints[JointType.WristLeft].Position.Z.ToString() + System.Environment.NewLine;
                    File.AppendAllText(filePath, str);
                    //Console.Write("|X1: " + skeletonData[0].Joints[JointType.Head].Position.X.ToString() +
                    //    " Y1: " + skeletonData[0].Joints[JointType.Head].Position.Y.ToString() +
                    //    " Z1: " + skeletonData[0].Joints[JointType.Head].Position.Z.ToString());

                    //Console.Write("|X3: " + skeletonData[0].Joints[JointType.HandLeft].Position.X.ToString() +
                    //    " Y3: " + skeletonData[0].Joints[JointType.HandLeft].Position.Y.ToString() +
                    //    " Z3: " + skeletonData[0].Joints[JointType.HandLeft].Position.Z.ToString());

                    //Console.WriteLine("|X4: " + skeletonData[0].Joints[JointType.WristLeft].Position.X.ToString() +
                    //    " Y4: " + skeletonData[0].Joints[JointType.WristLeft].Position.Y.ToString() +
                    //    " Z4: " + skeletonData[0].Joints[JointType.WristLeft].Position.Z.ToString());

                    // Console.Write("|X5: " + skeletonData[0].Joints[JointType.HandRight].Position.X.ToString() +
                    //     " Y5: " + skeletonData[0].Joints[JointType.HandRight].Position.Y.ToString() +
                    //     " Z5: " + skeletonData[0].Joints[JointType.HandRight].Position.Z.ToString());

                    // Console.Write("|X6: " + skeletonData[0].Joints[JointType.WristRight].Position.X.ToString() +
                    //     " Y6: " + skeletonData[0].Joints[JointType.WristRight].Position.Y.ToString() +
                    //     " Z6: " + skeletonData[0].Joints[JointType.WristRight].Position.Z.ToString());

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
}



