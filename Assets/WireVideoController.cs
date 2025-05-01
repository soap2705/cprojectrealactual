using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class WireVideoController : MonoBehaviour

    {
        public VideoClip[] videoClips; // Assign 2 clips in inspector: 0=Intro, 1=Background loop
        private VideoPlayer videoPlayer;
        private MeshRenderer meshRenderer;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = meshRenderer;
            videoPlayer.targetMaterialProperty = "_BaseMap"; // Change to _BaseMap

            if (videoClips != null && videoClips.Length > 0 && videoClips[0] != null)
            {
                PlayIntroVideo();
            }
            else
            {
                Debug.LogError("Video clips array is empty or intro clip not assigned.");
            }
        }

    void PlayIntroVideo()
    {
        videoPlayer.clip = videoClips[0];
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached -= OnBackgroundVideoLoop; // Ensure not subscribed
        videoPlayer.loopPointReached += OnIntroVideoFinished;

        // Prepare the video before playing
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnIntroVideoPrepared; // Subscribe to prepare completed
        Debug.Log("Preparing intro video: " + videoPlayer.clip.name);
    }

    void OnIntroVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnIntroVideoPrepared; // Unsubscribe from prepare completed
        vp.Play(); // Now play the video
        Debug.Log("Playing intro video: " + vp.clip.name);
    }

    void OnIntroVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnIntroVideoFinished; // Unsubscribe intro event
        PlayBackgroundLoop();
    }


    void PlayBackgroundLoop()
    {
        if (videoClips.Length > 1 && videoClips[1] != null)
        {
            videoPlayer.clip = videoClips[1];
            videoPlayer.isLooping = false;
            videoPlayer.loopPointReached -= OnIntroVideoFinished;
            videoPlayer.loopPointReached += OnBackgroundVideoLoop;

            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnBackgroundVideoPrepared;

            Debug.Log("Preparing background video: " + videoClips[1].name);
        }
        else
        {
            Debug.LogWarning("Background video clip not assigned, stopping playback.");
            videoPlayer.Stop();
        }
    }

    void OnBackgroundVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnBackgroundVideoPrepared;
        vp.Play();
        Debug.Log("Playing background video: " + vp.clip.name);
    }

    void OnBackgroundVideoLoop(VideoPlayer vp)
    {
        vp.loopPointReached -= OnBackgroundVideoLoop;
        StartCoroutine(RestartVideoAfterFrame(vp));
    }

    IEnumerator RestartVideoAfterFrame(VideoPlayer vp)
    {
        vp.Stop();
        yield return null; // Wait one frame
        vp.Play();
        vp.loopPointReached += OnBackgroundVideoLoop;
    }
}