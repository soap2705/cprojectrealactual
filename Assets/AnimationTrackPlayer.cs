using UnityEngine;
using System.Collections;

public class AnimationTrackPlayer : MonoBehaviour
{
    public GameObject[] spritePrefabs; // Array of different sprite prefabs to spawn
    private GameObject currentSprite;

    public void ActivateAnimationPath(Vector3 leftPointPosition, Vector3 rightPointPosition)
    {
        // Destroy the current sprite if it exists
        if (currentSprite != null)
        {
            Destroy(currentSprite);
        }

        // Instantiate a new sprite at the left point position
        currentSprite = Instantiate(GetRandomSpritePrefab(), leftPointPosition, Quaternion.identity);

        // Start moving the sprite along the path
        StartCoroutine(MoveSprite(currentSprite.transform, leftPointPosition, rightPointPosition));
    }

    // Method to get a random sprite prefab from the array
    private GameObject GetRandomSpritePrefab()
    {
        if (spritePrefabs == null || spritePrefabs.Length == 0)
        {
            Debug.LogError("No sprite prefabs assigned!");
            return null; // Return null if no prefabs are assigned
        }
        int randomIndex = Random.Range(0, spritePrefabs.Length);
        return spritePrefabs[randomIndex];
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