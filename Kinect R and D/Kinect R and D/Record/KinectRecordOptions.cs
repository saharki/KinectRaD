using System;

namespace Kinect_R_and_D.Record
{
    [FlagsAttribute]
    public enum KinectRecordOptions
    {
        Color = 1,
        Depth = 2,
        Skeletons = 4
    }
}
 