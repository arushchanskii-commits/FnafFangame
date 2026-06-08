using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MiniGameSceneEntry
{
    [Tooltip("Scene name for the next minigame.")]
    public string sceneName;

    [Tooltip("If true, this minigame scene will stay loaded when the next one starts.")]
    public bool stayLoaded;

    [Tooltip("Optional time limit in seconds before the manager advances to the next stage automatically.")]
    public float timeLimitSeconds;
}

public class MiniGameSwapper : MonoBehaviour
{
    [Header("Scene-based MiniGame Sequence")]
    [Tooltip("Assign the minigame scenes in the order they should play. Mark scenes to stay loaded when needed.")]
    [SerializeField]
    private List<MiniGameSceneEntry> miniGameSceneEntries = new List<MiniGameSceneEntry>();

    private int currentIndex;
    private static MiniGameSwapper instance;
    private readonly List<string> loadedSceneNames = new List<string>();
    private Coroutine sceneTimerCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (miniGameSceneEntries == null || miniGameSceneEntries.Count == 0)
        {
            Debug.LogWarning("MiniGameSwapper: No minigame scenes assigned.");
            return;
        }

        LoadSceneEntry(currentIndex, true);
    }

    private void LoadSceneEntry(int index, bool useSingle)
    {
        if (index >= miniGameSceneEntries.Count)
        {
            Debug.Log("MiniGameSwapper: All minigames completed.");
            return;
        }

        var entry = miniGameSceneEntries[index];
        if (entry == null || string.IsNullOrEmpty(entry.sceneName))
        {
            Debug.LogWarning($"MiniGameSwapper: Scene entry at index {index} is invalid. Skipping.");
            currentIndex++;
            if (currentIndex < miniGameSceneEntries.Count)
            {
                LoadSceneEntry(currentIndex, false);
            }
            return;
        }

        StopSceneTimer();
        var loadMode = useSingle ? LoadSceneMode.Single : LoadSceneMode.Additive;
        SceneManager.LoadScene(entry.sceneName, loadMode);
        if (!loadedSceneNames.Contains(entry.sceneName))
        {
            loadedSceneNames.Add(entry.sceneName);
        }

        if (entry.timeLimitSeconds > 0f)
        {
            StartSceneTimer(entry.timeLimitSeconds);
        }
    }

    public void OnMiniGameComplete()
    {
        StopSceneTimer();

        if (currentIndex < 0 || currentIndex >= miniGameSceneEntries.Count)
        {
            return;
        }

        var completedEntry = miniGameSceneEntries[currentIndex];
        currentIndex++;

        if (currentIndex >= miniGameSceneEntries.Count)
        {
            Debug.Log("MiniGameSwapper: All minigames completed.");
            return;
        }

        LoadSceneEntry(currentIndex, false);

        if (completedEntry != null && !completedEntry.stayLoaded)
        {
            UnloadSceneIfLoaded(completedEntry.sceneName);
        }
    }

    private void StartSceneTimer(float seconds)
    {
        StopSceneTimer();
        sceneTimerCoroutine = StartCoroutine(SceneTimerCoroutine(seconds));
    }

    private void StopSceneTimer()
    {
        if (sceneTimerCoroutine != null)
        {
            StopCoroutine(sceneTimerCoroutine);
            sceneTimerCoroutine = null;
        }
    }

    private IEnumerator SceneTimerCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentIndex >= 0 && currentIndex < miniGameSceneEntries.Count)
        {
            Debug.Log($"MiniGameSwapper: Time limit reached for {miniGameSceneEntries[currentIndex].sceneName}. Advancing automatically.");
        }
        CompleteCurrentMiniGame();
    }

    private void UnloadSceneIfLoaded(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return;
        }

        if (loadedSceneNames.Contains(sceneName))
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
            loadedSceneNames.Remove(sceneName);
        }
    }

    public static void CompleteCurrentMiniGame()
    {
        if (instance == null)
        {
            Debug.LogWarning("MiniGameSwapper: No instance found. Make sure a MiniGameSwapper exists in the first scene.");
            return;
        }

        instance.OnMiniGameComplete();
    }
}
