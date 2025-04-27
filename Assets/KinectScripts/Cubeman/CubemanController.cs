using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CubemanController : MonoBehaviour
{
    public bool MoveVertically = false;
    public bool MirroredMovement = false;

    public PointSpawner pointSpawner;

    //public GameObject debugText;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Shoulder_Center;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;

    public LineRenderer SkeletonLine;

    private GameObject[] bones;
    private LineRenderer[] lines;
    private int[] parIdxs;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialPosOffset = Vector3.zero;
    private uint initialPosUserID = 0;


    // For joint distances
    public float leftShoulderToElbowDistance;
    public float leftElbowToWristDistance;
    public float leftHandtoWristDistance;
    public float rightShoulderToElbowDistance;
    public float rightElbowToWristDistance;
    public float righthandtoWristDistance;
    public float leftshoulderToCenter;
    public float rightShoulderToCenter;

    //for the animation path
    public GameObject spritePrefab;
    private GameObject currentSprite;
    public float animationSpeed = 1.0f;
    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentPathIndex = 0;


    void Start()
    {
        //store bones in a list for easier access
        bones = new GameObject[] {
            Hip_Center, Spine, Shoulder_Center, Head,  // 0 - 3
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,  // 4 - 7
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,  // 8 - 11
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,  // 12 - 15
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right  // 16 - 19
		};

        parIdxs = new int[] {
            0, 0, 1, 2,
            2, 4, 5, 6,
            2, 8, 9, 10,
            0, 12, 13, 14,
            0, 16, 17, 18
        };

        // array holding the skeleton lines
        lines = new LineRenderer[bones.Length];

        if (SkeletonLine)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Instantiate(SkeletonLine) as LineRenderer;
                lines[i].transform.parent = transform;
            }
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //transform.rotation = Quaternion.identity;

        if (currentSprite == null)
        {
            currentSprite = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        // get 1st player
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;

        if (playerID <= 0)
        {
            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;

                if (SkeletonLine)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
            return;

        }

        // set the user position in space
        Vector3 posPointMan = manager.GetUserPosition(playerID);
        posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;

        // store the initial position
        if (initialPosUserID != playerID)
        {
            initialPosUserID = playerID;
            initialPosOffset = transform.position - (MoveVertically ? posPointMan : new Vector3(posPointMan.x, posPointMan.y, posPointMan.z));
        }

        transform.position = initialPosOffset + (MoveVertically ? posPointMan : new Vector3(posPointMan.x, posPointMan.y, posPointMan.z));

        // Set the position
        transform.position = new Vector3(
            initialPosOffset.x + (MoveVertically ? posPointMan.x : posPointMan.x),
            initialPosOffset.y + (MoveVertically ? posPointMan.y : 0),
            initialPosOffset.z + (MoveVertically ? posPointMan.z : 0)

        );


        // Set the position
        transform.position = new Vector3(
            initialPosOffset.x + (MoveVertically ? posPointMan.x : posPointMan.x),
           -1.3f, 0f // Fixed y + z position
        );

        // Calculate + scale user height based on hip to head data 
        Vector3 hipLeftPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HipLeft);
        Vector3 headPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.Head);
        float height = (Vector3.Distance(hipLeftPos, headPos)) * 6f; // Height from hip to head multiplied based on distance from the kinect
        float scaleFactor = height; // Use height directly for scaling
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);


        //Update the values for the arms based on individual lengths of bones
        Vector3 leftWristPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
        Vector3 leftElbowPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
        Vector3 leftShoulderPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
        Vector3 rightShoulderPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
        Vector3 rightElbowPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);
        Vector3 rightWristPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.WristRight);
        Vector3 shouldercenterPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderCenter);
        Vector3 rightHandPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandRight);
        Vector3 leftHandPos = manager.GetJointPosition(playerID, (int)KinectWrapper.NuiSkeletonPositionIndex.HandLeft);


        // Calculate distances
        leftShoulderToElbowDistance = Vector3.Distance(leftShoulderPos, leftElbowPos);
        leftElbowToWristDistance = Vector3.Distance(leftElbowPos, leftWristPos);
        rightShoulderToElbowDistance = Vector3.Distance(rightShoulderPos, rightElbowPos);
        rightElbowToWristDistance = Vector3.Distance(rightElbowPos, rightWristPos);
        leftshoulderToCenter = Vector3.Distance(leftShoulderPos, shouldercenterPos);
        rightShoulderToCenter = Vector3.Distance(rightShoulderPos, shouldercenterPos);
        righthandtoWristDistance = Vector3.Distance(rightHandPos, rightWristPos);
        leftHandtoWristDistance = Vector3.Distance(leftHandPos, leftWristPos);


        // Update the local positions of the bones 
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i) : i;

                if (manager.IsJointTracked(playerID, joint))
                {
                    bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(playerID, joint);
                    posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;

                    Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, !MirroredMovement);
                    rotJoint = initialRotation * rotJoint;

                    posJoint -= posPointMan;

                    if (MirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }

                    bones[i].transform.localPosition = posJoint;
                    bones[i].transform.rotation = rotJoint;
                }
                else
                {
                    bones[i].gameObject.SetActive(false);
                }
            }
        }



        // Update the skeleton lines
        if (SkeletonLine)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                bool bLineDrawn = false;

                if (bones[i] != null)
                {
                    if (bones[i].gameObject.activeSelf)
                    {
                        Vector3 posJoint = bones[i].transform.position;

                        int parI = parIdxs[i];
                        Vector3 posParent = bones[parI].transform.position;

                        if (bones[parI].gameObject.activeSelf)
                        {
                            lines[i].gameObject.SetActive(true);

                            lines[i].SetPosition(0, posParent);
                            lines[i].SetPosition(1, posJoint);

                            bLineDrawn = true;
                        }
                    }
                }

                if (!bLineDrawn)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
        }


        // Check if the hands are within the bounds of the points
        if (pointSpawner != null && pointSpawner.leftPointTouched && pointSpawner.rightPointTouched)
        {
            // Move the sprite to the midpoint between the hands
            if (currentSprite != null)
            {
                currentSprite.transform.position = Vector3.Lerp(leftHandPos, rightHandPos, 0.5f);
            }
        }

        // Calculate the path points based on the bone positions
        CalculatePathPoints();

        // Move the sprite along the path
        MoveSpriteAlongPath();
    }

    private void CalculatePathPoints()
    {
        pathPoints.Clear(); // Clear previous points

        // Get positions of the relevant joints (you can choose which bones to use)
        Vector3 leftShoulderPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
        Vector3 leftElbowPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.ElbowLeft);
        Vector3 leftWristPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.WristLeft);
        Vector3 leftHandPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.HandLeft);
        Vector3 rightShoulderPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);
        Vector3 rightElbowPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.ElbowRight);
        Vector3 rightWristPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.WristRight);
        Vector3 rightHandPos = GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex.HandRight);

        // Add the positions to the path points
        pathPoints.Add(leftShoulderPos);
        pathPoints.Add(leftElbowPos);
        pathPoints.Add(leftWristPos);
        pathPoints.Add(leftHandPos);
        pathPoints.Add(rightShoulderPos);
        pathPoints.Add(rightElbowPos);
        pathPoints.Add(rightWristPos);
        pathPoints.Add(rightHandPos);
    }

    private Vector3 GetJointPosition(KinectWrapper.NuiSkeletonPositionIndex jointIndex)
    {
        KinectManager manager = KinectManager.Instance;
        uint playerID = manager != null ? manager.GetPlayer1ID() : 0;
        return manager.GetJointPosition(playerID, (int)jointIndex);
    }

    private bool shouldMoveSprite = false; // Flag to control sprite movement

    public void StartMovingSprite()
    {
        shouldMoveSprite = true; // Set the flag to true to start moving the sprite
    }

    private void MoveSpriteAlongPath()
    {
        if (pathPoints.Count == 0 || currentSprite == null || !shouldMoveSprite) return;

        // Move the sprite towards the current path point
        Vector3 targetPosition = pathPoints[currentPathIndex];
        currentSprite.transform.position = Vector3.MoveTowards(currentSprite.transform.position, targetPosition, animationSpeed * Time.deltaTime);

        // Check if the sprite has reached the target position
        if (Vector3.Distance(currentSprite.transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= pathPoints.Count)
            {
                currentPathIndex = 0; // Loop back to the start
            }
        }
    }
}


