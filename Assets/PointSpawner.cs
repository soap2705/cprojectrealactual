using System.Collections.Generic; // Required for List
using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    public GameObject BboxL; // Left bounding box
    public GameObject BboxR; // Right bounding box
    public GameObject pointPrefab; // Prefab for the point to spawn
    public float sphereDiameter = 0.5f; // Diameter for the debug spheres
    public float minVerticalOffset = 1.0f; // Minimum vertical offset between the two points

    private List<GameObject> points = new List<GameObject>(); // List to hold references to spawned points
    private GameObject leftPoint;
    private GameObject rightPoint;
    public bool leftPointTouched = false;
    public bool rightPointTouched = false;

    // Reference to the animation path script
    public AnimationTrackPlayer animationPathScript; // Assign this in the Inspector

    // Call this method when a player is detected
    public void OnPlayerDetected()
    {
        SpawnPoints();
    }

    // Call this method when a player is no longer detected
    public void OnPlayerLost()
    {
        Debug.Log("Player lost, resetting points...");
        ResetPoints();
    }

    private void ResetPoints()
    {
        Debug.Log("Resetting points...");
        foreach (var point in points)
        {
            if (point != null)
            {
                Debug.Log($"Destroying point: {point.name}");
                point.SetActive(false);
                Destroy(point);
                Debug.Log($"Point {point.name} active status: {point.activeSelf}"); // Check active status
            }
        }
        points.Clear();
        leftPointTouched = false;
        rightPointTouched = false;
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

        // Add the instantiated points to the list
        points.Add(leftPoint);
        points.Add(rightPoint);

        // Add colliders to the points
        leftPoint.AddComponent<SphereCollider>().isTrigger = true;
        rightPoint.AddComponent<SphereCollider>().isTrigger = true;

        // Log the positions to the console
        Debug.Log("Left Point Position: " + leftPointPosition);
        Debug.Log("Right Point Position: " + rightPointPosition);

        // Create debug spheres with a specific diameter
        CreateDebugSphere(leftPointPosition, sphereDiameter);
        CreateDebugSphere(rightPointPosition, sphereDiameter);
    }

    // Method to get the position of the left point
    public Vector3 GetLeftPointPosition()
    {
        if (leftPoint != null)
        {
            return leftPoint.transform.position;
        }
        return Vector3.zero; // Return zero if the left point is not available
    }
    private Vector3 GetRandomPoint(GameObject plane)
    {
        Vector3 position = plane.transform.position;
        Vector3 size = plane.transform.localScale;

        // Align x and z with the center of the bounding box
        float x = position.x;
        float z = position.z;

        // Randomize y within the bounding box, but also add a larger range
        float yMin = position.y - (size.y / 2);
        float yMax = position.y + (size.y / 2);
        float y = Random.Range(yMin - 5.0f, yMax + 5.0f);

        return new Vector3(x, y, z);
    }

    private void CreateDebugSphere(Vector3 position, float diameter)
    {
        // Create a small sphere to visualize the point
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.transform.position = position;

        // Set the scale based on the desired diameter
        float radius = diameter / 2f; // Calculate the radius
        debugSphere.transform.localScale = new Vector3(radius, radius, radius);

        // Change color for visibility
        debugSphere.GetComponent<Renderer>().material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get the player's hand positions
        KinectManager manager = KinectManager.Instance;
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        if (playerID > 0)
        {
            Vector3 leftHandPosition = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
            Vector3 rightHandPosition = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);

            // Check if the left point is touched
            if (other.gameObject.CompareTag("LeftPoint"))
            {
                Debug.Log("Left point touched!");
                leftPointTouched = true;
                other.gameObject.GetComponent<Renderer>().material.color = Color.green; // Change color to green when touched
                CheckWinCondition(leftHandPosition, rightHandPosition); // Pass hand positions
            }

            // Check if the right point is touched
            if (other.gameObject.CompareTag("RightPoint"))
            {
                Debug.Log("Right point touched!");
                rightPointTouched = true;
                other.gameObject.GetComponent<Renderer>().material.color = Color.green; // Change color to green when touched
                CheckWinCondition(leftHandPosition, rightHandPosition); // Pass hand positions
            }
        }
    }


    private void CheckWinCondition(Vector3 playerLeftHandPosition, Vector3 playerRightHandPosition)
    {
        // Check if the player's hands are within the bounds of the left and right points
        if (leftPointTouched && rightPointTouched)
        {
            Vector3 leftPointPosition = points[0].transform.position; // Assuming left point is at index 0
            Vector3 rightPointPosition = points[1].transform.position; // Assuming right point is at index 1

            // Check if player's hands are within the X and Y bounds of the points
            if (playerLeftHandPosition.x >= leftPointPosition.x - (sphereDiameter / 2) &&
                playerLeftHandPosition.x <= leftPointPosition.x + (sphereDiameter / 2) &&
                playerLeftHandPosition.y >= leftPointPosition.y - (sphereDiameter / 2) &&
                playerLeftHandPosition.y <= leftPointPosition.y + (sphereDiameter / 2) &&
                playerRightHandPosition.x >= rightPointPosition.x - (sphereDiameter / 2) &&
                playerRightHandPosition.x <= rightPointPosition.x + (sphereDiameter / 2) &&
                playerRightHandPosition.y >= rightPointPosition.y - (sphereDiameter / 2) &&
                playerRightHandPosition.y <= rightPointPosition.y + (sphereDiameter / 2))
            {
                Debug.Log("Win condition met! Activating animation path script...");

                // Call the method to activate the animation and pass the point positions
                animationPathScript.ActivateAnimationPath(leftPointPosition, rightPointPosition);

                ResetPoints(); // Reset points after activation
            }
        }
    }
}