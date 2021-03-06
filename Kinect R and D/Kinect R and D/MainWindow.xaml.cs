﻿using System;
using System.IO;
using System.Windows;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Timers;
using Kinect_R_and_D.Record;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Kinect_R_and_D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;
        private KinectSensor kinect;
        private SkeletonPositionTracking skeleonTracker;
        private const string fileName = "ColorVideo.wmv";

        private ColorRecorder colorDisplay;
        private DepthRecoder depthDisplay;


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            //SetTimer();
            // Utility.SetTimer(15000);
        }



        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            //Initialize sensor UI helper called sensorChooser and start the sensor.
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += this.SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
            this.kinect = this.sensorChooser.Kinect;

            /* Skeleton handling */

            // Allocate and start the skeleton Data stream.
            this.skeleonTracker = new SkeletonPositionTracking(kinect.SkeletonStream.FrameSkeletonArrayLength);
            this.kinect.SkeletonStream.Enable();

            // Get Ready for Skeleton Ready Events.
            this.kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(skeleonTracker.kinectSkeletonFrameReady);
            /* End Skeleton handling */

            /* Color handling */
            this.colorDisplay = new ColorRecorder(kinect);

            // Turn on the color stream to receive color frames
            this.kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            // Set the image we display to point to the bitmap where we'll put the image data
            this.colorImage.Source = colorDisplay.ColorBitmap;

            // Add an event handler to be called whenever there is new color frame data
            this.kinect.ColorFrameReady += this.colorDisplay.SensorColorFrameReady;
            /* End of Color handling. */

            /* Depth handling */
            this.depthDisplay = new DepthRecoder();

            // Turn on the depth stream to receive depth frames
            this.kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

            // Allocate space to put the depth pixels we'll receive
            depthDisplay.DepthPixels = new DepthImagePixel[this.kinect.DepthStream.FramePixelDataLength];

            // Allocate space to put the color pixels we'll create
            depthDisplay.ColorPixels = new byte[this.kinect.DepthStream.FramePixelDataLength * sizeof(int)];

            // This is the bitmap we'll display on-screen
            depthDisplay.ColorBitmap = new WriteableBitmap(this.kinect.DepthStream.FrameWidth, this.kinect.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

            // Set the image we display to point to the bitmap where we'll put the image data
            this.depthImage.Source = depthDisplay.ColorBitmap;

            // Add an event handler to be called whenever there is new depth frame data
            this.kinect.DepthFrameReady += depthDisplay.SensorDepthFrameReady;
            /* End of Depth handling */

            this.kinect.Start();
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
                    e.OldSensor.ColorStream.Disable();
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
                    e.NewSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

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