using UnityEngine;
using System;
using System.Collections;
public class AnimationTrackPlayer : MonoBehaviour
{
    //references the player controller script that is publicly broadcasting the distances of players arms
    public CubemanController cubemanController;
    public PointSpawner spawner;

    void Start()
    {
        // Ensure the reference is set
        if (cubemanController == null)
        {
            Debug.LogError("JointDistanceScript reference is not set!");
            return;
        }
    }

   
    // Update is called once per frame
    void Update()
    {
        float leftShoulderToElbow = cubemanController.leftShoulderToElbowDistance;
        float leftElbowToWrist = cubemanController.leftElbowToWristDistance;
        float leftHandToWrist = cubemanController.leftHandtoWristDistance;
        float rightShoulderToElbow = cubemanController.rightShoulderToElbowDistance;
        float rightElbowToWrist = cubemanController.rightElbowToWristDistance;
        float rightHandToWrist = cubemanController.righthandtoWristDistance;
        float leftShoulderToCenter = cubemanController.leftshoulderToCenter;
        float rightShoulderToCenter = cubemanController.rightShoulderToCenter;
    }
}
