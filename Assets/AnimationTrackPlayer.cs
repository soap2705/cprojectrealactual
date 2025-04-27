using UnityEngine;
using System;
using System.Collections;
public class AnimationTrackPlayer : MonoBehaviour
{
    public GameObject spritePrefab;
    public PointSpawner pointSpawner;
    private GameObject currentSprite;

    public void ActivateAnimationPath(Vector3 leftPointPosition, Vector3 rightPointPosition)
    {
        // Instantiate the sprite if not already instantiated
        if (currentSprite == null)
        {
            currentSprite = Instantiate(spritePrefab, leftPointPosition, Quaternion.identity); // Position at left point
        }

        // Start moving the sprite along the path
        StartCoroutine(MoveSprite(currentSprite.transform, leftPointPosition, rightPointPosition));
    }

    private IEnumerator MoveSprite(Transform spriteTransform, Vector3 startPoint, Vector3 endPoint)
    {
        float duration = 2.0f; // Duration to move from point A to B
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the fraction of the journey completed
            float t = elapsedTime / duration;

            // Interpolate the position of the sprite
            spriteTransform.position = Vector3.Lerp(startPoint, endPoint, t);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the sprite ends exactly at the end point
        spriteTransform.position = endPoint;
    }
}