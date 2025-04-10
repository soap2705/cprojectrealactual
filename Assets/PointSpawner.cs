
using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    public GameObject BboxL; // Left bounding box
    public GameObject BboxR; // Right bounding box
    public GameObject pointPrefab; // Prefab for the point to spawn
    public float sphereDiameter = 0.5f; // Diameter for the debug spheres
    public float minVerticalOffset = 1.0f; // Minimum vertical offset between the two points

    private GameObject leftPoint; // Reference to the left point
    private GameObject rightPoint; // Reference to the right point

    // Call this method when a player is detected
    public void OnPlayerDetected()
    {
        SpawnPoints();
    }

    // Call this method when a player is no longer detected
    public void OnPlayerLost()
    {
        ResetPoints();
    }

    void SpawnPoints()
    {
        // Generate random points
        Vector3 leftPointPosition = GetRandomPoint(BboxL);
        Vector3 rightPointPosition = GetRandomPoint(BboxR);

        // Ensure the vertical positions are within the bounding box limits
        float leftYMin = BboxL.transform.position.y - (BboxL.transform.localScale.y / 2);
        float leftYMax = BboxL.transform.position.y + (BboxL.transform.localScale.y / 2);
        float rightYMin = BboxR.transform.position.y - (BboxR.transform.localScale.y / 2);
        float rightYMax = BboxR.transform.position.y + (BboxR.transform.localScale.y / 2);

        // Generate a new random y for the right point within its bounding box
        rightPointPosition.y = Random.Range(rightYMin, rightYMax);

        // Ensure the right point is at least minVerticalOffset above the left point
        if (rightPointPosition.y < leftPointPosition.y + minVerticalOffset)
        {
            rightPointPosition.y = leftPointPosition.y + minVerticalOffset;
        }

        // Clamp the right point's y-coordinate to ensure it stays within its bounding box
        rightPointPosition.y = Mathf.Clamp(rightPointPosition.y, rightYMin, rightYMax);

        // Instantiate the point prefabs
        leftPoint = Instantiate(pointPrefab, leftPointPosition, Quaternion.identity);
        rightPoint = Instantiate(pointPrefab, rightPointPosition, Quaternion.identity);

        // Log the positions to the console
        Debug.Log("Left Point Position: " + leftPointPosition);
        Debug.Log("Right Point Position: " + rightPointPosition);

        // Create debug spheres with a specific diameter
        CreateDebugSphere(leftPointPosition, sphereDiameter);
        CreateDebugSphere(rightPointPosition, sphereDiameter);
    }


    private void ResetPoints()
    {
        UnityEngine.Debug.Log("Resetting points...");
        if (leftPoint != null)
        {
            GameObject.Destroy(leftPoint); // Correctly call Destroy on GameObject
            leftPoint = null; // Reset reference
            UnityEngine.Debug.Log("Left point destroyed.");
        }
        if (rightPoint != null)
        {
            GameObject.Destroy(rightPoint); // Correctly call Destroy on GameObject
            rightPoint = null; // Reset reference
            UnityEngine.Debug.Log("Right point destroyed.");
        }
    }

    private Vector3 GetRandomPoint(GameObject plane)
    {
        Vector3 position = plane.transform.position;
        Vector3 size = plane.transform.localScale;

        // Align x and z with the center of the bounding box
        float x = position.x; // Center x of the bounding box
        float z = position.z; // Center z of the bounding box

        // Randomize y within the bounding box, but also add a larger range
        float yMin = position.y - (size.y / 2);
        float yMax = position.y + (size.y / 2);
        float y = Random.Range(yMin - 5.0f, yMax + 5.0f); // Adjust the offset as needed

        return new Vector3(x, y, z);
    }
    private void CreateDebugSphere(Vector3 position, float diameter)
    {
        // Create a small sphere to visualize the point
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = position;

        // Set the scale based on the desired diameter
        float radius = diameter / 2f; // Calculate the radius
        debugSphere.transform.localScale = new Vector3(radius, radius, radius); // Set the scale

        // Change color for visibility
        debugSphere.GetComponent<Renderer>().material.color = Color.red;
    }
}