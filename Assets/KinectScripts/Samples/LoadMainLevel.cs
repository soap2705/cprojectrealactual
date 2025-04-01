using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainLevel : MonoBehaviour 
{
	public bool levelLoaded = false;
	public int scenenumber;

	public SimpleGestureListener gesture;

    private void Awake()
    {
        gesture = FindFirstObjectByType<SimpleGestureListener>();
    }

    void Update() 
	{
		KinectManager manager = KinectManager.Instance;

        if (!levelLoaded && manager && KinectManager.IsKinectInitialized())
		{
            //if (gesture.IsRiseRightHand())
            {
                levelLoaded = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
