using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    public GameObject BboxL;
    public GameObject BboxR;
    public GameObject PointAPointBSpawner;

    private void Start()
    {
        SpawnPoints();
    }

    void SpawnPoints()
    {
        Vector3 leftPoint = GetRandomPoint(BboxL);
        Vector3 rightPoint = GetRandomPoint(BboxR);

        Instantiate(PointAPointBSpawner, leftPoint, Quaternion.identity);
        Instantiate(PointAPointBSpawner, rightPoint, Quaternion.identity);


        // Log the positions to the console
        Debug.Log("Left Point Position: " + leftPoint);
        Debug.Log("Right Point Position: " + rightPoint);

        // Create debug spheres with a specific diameter (e.g., 0.5 units)
        float sphereDiameter = 0.5f; // Set your desired diameter here
        CreateDebugSphere(leftPoint, sphereDiameter);
        CreateDebugSphere(rightPoint, sphereDiameter);
    }

    private Vector3 GetRandomPoint(GameObject plane)
    {
        Vector3 position = plane.transform.position;
        Vector3 size = plane.transform.localScale;

        float x = Random.Range(position.x - size.x / 2, position.x + size.x / 2);
        float y = position.y;
        float z = Random.Range(position.z - size.z / 2, position.z + size.z / 2);

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
