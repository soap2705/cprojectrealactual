using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 
      videoPlayer = GetComponent<VideoPlayer>();
      if (videoPlayer != null)
        {
          videoPlayer.Play();
            }
        }
    }

    // Update is called once per frame

