using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 
using UnityEngine.UI;

public class LevelTransitions : MonoBehaviour
{
    public static LevelTransitions Instance; 

    [Header("Progression Scenes - will be loaded in order")]
    public string[] progressionScenes = new string[3];

    [Header("Final Scenes - randomly pick one")]
    public string[] finalScenes;

    [Header("Fade Controller (Assign your UI Fade panel controller here)")]
    public SceneFade fadeController; 

    private int currentProgressIndex = 0;
    private const string ProgressIndexKey = "ProgressSceneIndex";

    void Start()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved progression index or start at 0
        currentProgressIndex = PlayerPrefs.GetInt(ProgressIndexKey, 0);
    }

    public void LoadNextScene()
    {
        if (currentProgressIndex < progressionScenes.Length)
        {
            StartCoroutine(LoadSceneWithFade(progressionScenes[currentProgressIndex]));
            currentProgressIndex++;
            PlayerPrefs.SetInt(ProgressIndexKey, currentProgressIndex);
            PlayerPrefs.Save();
        }
        else
        {
            if (finalScenes == null || finalScenes.Length == 0)
            {
                Debug.LogError("Final scenes array is empty, cannot load final scene!");
                return;
            }
            string randomFinal = finalScenes[Random.Range(0, finalScenes.Length)];
            StartCoroutine(LoadSceneWithFade(randomFinal));
        }
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        if (fadeController != null)
        {
            yield return fadeController.FadeOut();
        }
        else
        {
            Debug.LogWarning("FadeController not assigned! Scene will load without fade.");
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (fadeController != null)
        {
            yield return fadeController.FadeIn();
        }
    }
}
