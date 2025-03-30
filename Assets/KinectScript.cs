using UnityEngine;
using Microsoft.Kinect;
using Unity.VisualScripting;
using System.Linq;
using System;

public class KinectScript : MonoBehaviour
{
    private KinectSensor kinect;
    private Skeleton[] skeletonData;

    void Start()
    {
        StartKinectST();
    }

    void StartKinectST()
    {
        // Get the first connected Kinect sensor
        kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

        if (kinect != null)
        {
            // Enable skeletal tracking
            kinect.SkeletonStream.Enable();

            // Allocate space for skeleton data
            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];

            // Subscribe to the SkeletonFrameReady event
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);

            // Start the Kinect sensor
            kinect.Start();
        }
        else
        {
            Debug.LogError("No Kinect sensor connected.");
        }
    }

    void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
    {
        // Get the skeleton frame
        using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
        {
            if (skeletonFrame != null)
            {
                // Copy skeleton data to the skeletonData array
                skeletonFrame.CopySkeletonDataTo(skeletonData);

                // Process each skeleton
                foreach (Skeleton skeleton in skeletonData)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        // Example: Access joint positions
                        Vector3 headPosition = new Vector3(skeleton.Joints[JointType.Head].Position.X,
                                                           skeleton.Joints[JointType.Head].Position.Y,
                                                           skeleton.Joints[JointType.Head].Position.Z);
                        // Do something with the head position, e.g., log it
                        Debug.Log("Head Position: " + headPosition);
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (kinect != null)
        {
            // Stop the Kinect sensor
            kinect.Stop();
        }
    }
}