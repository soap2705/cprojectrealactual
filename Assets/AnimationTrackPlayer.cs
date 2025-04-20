using UnityEngine;
using System;
using System.Collections;
public class AnimationTrackPlayer : MonoBehaviour
{
    public GameObject spritePrefab; // Prefab with SpriteRenderer and Animator
    public PointSpawner pointSpawner;
    public CubemanController cubemanController; // Reference to the CubemanController script
    private GameObject currentSprite; // The instantiated sprite

    public void ActivateAnimationPath()
    {
        // Access joint distance values from CubemanController
        float leftShoulderToElbow = cubemanController.leftShoulderToElbowDistance;
        float leftElbowToWrist = cubemanController.leftElbowToWristDistance;
        float leftHandToWrist = cubemanController.leftHandtoWristDistance;
        float rightShoulderToElbow = cubemanController.rightShoulderToElbowDistance;
        float rightElbowToWrist = cubemanController.rightElbowToWristDistance;
        float rightHandToWrist = cubemanController.righthandtoWristDistance;
        float leftShoulderToCenter = cubemanController.leftshoulderToCenter;
        float rightShoulderToCenter = cubemanController.rightShoulderToCenter;

        // Instantiate the sprite if not already instantiated
        if (currentSprite == null)
        {
            currentSprite = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity); // Position as needed
        }

        // Set animation parameters based on joint distances
        if (currentSprite != null)
        {
            Animator animator = currentSprite.GetComponent<Animator>();
            animator.SetFloat("lefthandtowrist", leftHandToWrist);
            animator.SetFloat("LeftElbowToWrist", leftElbowToWrist);
            animator.SetFloat("LeftShoulderToElbow", leftShoulderToElbow);
            animator.SetFloat("LeftShouldertoCenter", leftShoulderToCenter);
            animator.SetFloat("centertorightshoulder", rightShoulderToCenter);
            animator.SetFloat("RightShoulderToElbow", rightShoulderToElbow);
            animator.SetFloat("RightElbowToWrist", rightElbowToWrist);
            animator.SetFloat("righthandtowrist", rightHandToWrist);
         
        }

        // Trigger the animation
        if (currentSprite != null)
        {
            currentSprite.GetComponent<Animator>().SetTrigger("StartAnimation");
        }
    }
}