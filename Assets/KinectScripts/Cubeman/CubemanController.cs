using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;


public class CubemanController : MonoBehaviour
{
    public GameObject winVideoPlayer;
    public GameObject animationSprite;

    public bool MoveVertically = false;
    public bool MirroredMovement = true;

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



    //for the animation path
    public float animationSpeed = 1.0f;
    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentPathIndex = 0;

    public GameObject leftHandArea; // Assign in the inspector
    public GameObject rightHandArea; // Assign in the inspector

    private bool hasWon = false;



    void Start()
    {
        if (animationSprite != null)
        {
            animationSprite.SetActive(false);
        }

        if (winVideoPlayer != null)
        {
            winVideoPlayer.SetActive(false);
        }

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
                lines[i].enabled = false;
            }
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //transform.rotation = Quaternion.identity;

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
        // Call the UpdateWinState method
        UpdateWinState(manager, playerID);
    }


    private bool isAnimating = false; // flag to track ongoing animation
    private void UpdateWinState(KinectManager manager, uint playerID)
    {
        Debug.Log("UpdateWinState called");
        Vector3 leftWristPos = Wrist_Left.transform.position;
        Vector3 rightWristPos = Wrist_Right.transform.position;

        bool leftInArea = IsWristInArea(leftWristPos, leftHandArea);
        bool rightInArea = IsWristInArea(rightWristPos, rightHandArea);

        Debug.Log($"Left Wrist in Area: {leftInArea}");
        Debug.Log($"Right Wrist in Area: {rightInArea}");

        // Trigger win only when both wrists are in their respective areas
        if (leftInArea && rightInArea && !hasWon)
        {
            hasWon = true;
            Debug.Log("Win!");
            if (!isAnimating) // Only start if not already animating
            {
                StartCoroutine(TriggerAnimationWithDelay( 2.0f )); // 5-second delay
            }
        }
        else if (!leftInArea && !rightInArea)
        {
            if (hasWon)
            {
                Debug.Log("Win state reset.");
            }
            hasWon = false;
        }
    }

    private bool IsWristInArea(Vector3 wristWorldPos, GameObject area)
    {
        if (area == null)
        {
            Debug.LogWarning("Bounding area GameObject is not assigned.");
            return false;
        }

        BoxCollider boxCollider = area.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogWarning($"No BoxCollider component found on {area.name}");
            return false;
        }

        // Transform the world point to the local space of the collider's center
        Vector3 localPos = area.transform.InverseTransformPoint(wristWorldPos);

        // Adjust for BoxCollider center offset inside the GameObject
        Vector3 center = boxCollider.center;

        // Calculate local position relative to collider center
        Vector3 localPosRelativeToCenter = localPos - center;

        // Half size of the box collider (considering scale)
        Vector3 halfSize = Vector3.Scale(boxCollider.size * 0.5f, area.transform.localScale);

        // Check each axis if inside the half extents of the box collider
        bool insideX = localPosRelativeToCenter.x >= -halfSize.x && localPosRelativeToCenter.x <= halfSize.x;
        bool insideY = localPosRelativeToCenter.y >= -halfSize.y && localPosRelativeToCenter.y <= halfSize.y;
        bool insideZ = localPosRelativeToCenter.z >= -halfSize.z && localPosRelativeToCenter.z <= halfSize.z;

        bool isInside = insideX && insideY && insideZ;

        // Debug draw rays to visualize check
        Debug.DrawRay(wristWorldPos, Vector3.up * 0.3f, isInside ? Color.green : Color.red, 1.0f);

        return isInside;
    }

    private void OnDrawGizmos()
    {
        // Draw the character's position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f); // Small sphere at character's position

        // Draw the bounding boxes for the hand areas
        if (leftHandArea != null)
        {
            Gizmos.color = Color.green;
            BoxCollider box = leftHandArea.GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.matrix = leftHandArea.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
        if (rightHandArea != null)
        {
            Gizmos.color = Color.red;
            BoxCollider box = rightHandArea.GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.matrix = rightHandArea.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
        }
    }

    private IEnumerator TriggerAnimationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        StartCoroutine(PlayWristToWristAnimation()); // Start the animation
    }

    private Vector3[] GetAnimationPath()
    {
        return new Vector3[]
        {
        Wrist_Left.transform.position,
        Elbow_Left.transform.position,
        Shoulder_Left.transform.position,
        Shoulder_Right.transform.position,
        Elbow_Right.transform.position,
        Wrist_Right.transform.position
        };
    }

    private IEnumerator PlayWristToWristAnimation()
    {
        isAnimating = true;

        if (animationSprite != null)
        {
            animationSprite.SetActive(true);
        }

        if (winVideoPlayer != null)
        {
            winVideoPlayer.SetActive(true);
            var videoPlayer = winVideoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>();
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
        }

        Vector3 startPos = Wrist_Left.transform.position;
        Vector3 endPos = Wrist_Right.transform.position;

        float duration = 5.0f; // Animation duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

            if (animationSprite != null)
            {
                // Update position
                animationSprite.transform.position = currentPos;

                // Calculate direction toward the target (right wrist)
                Vector3 direction = (endPos - startPos).normalized;

                // Option 1: use LookRotation for rotation towards direction (assuming forward = Z axis)
                if (direction != Vector3.zero)
                {
                    animationSprite.transform.rotation = Quaternion.LookRotation(direction);
                }

                // Option 2: If your sprite uses different forward axis (e.g. up), adjust accordingly
                // animationSprite.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (animationSprite != null)
        {
            animationSprite.transform.position = endPos;
        }

        yield return new WaitForSeconds(0.5f);

        if (animationSprite != null)
        {
            animationSprite.SetActive(false);
        }

        if (winVideoPlayer != null)
        {
            var videoPlayer = winVideoPlayer.GetComponent<UnityEngine.Video.VideoPlayer>();
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
            winVideoPlayer.SetActive(false);
        }

        isAnimating = false;
    }

}