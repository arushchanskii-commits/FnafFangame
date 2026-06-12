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

        PrintConfiguredEntries();
        LoadSceneEntry(currentIndex, true);
    }

    private void PrintConfiguredEntries()
    {
        Debug.Log($"MiniGameSwapper: Printing {miniGameSceneEntries.Count} configured entries for debugging.");
        for (int i = 0; i < miniGameSceneEntries.Count; i++)
        {
            var e = miniGameSceneEntries[i];
            if (e == null)
            {
                Debug.LogWarning($"MiniGameSwapper: Entry {i} is null.");
                continue;
            }

            Debug.Log($"MiniGameSwapper: Entry {i} -> sceneName='{e.sceneName}', stayLoaded={e.stayLoaded}, timeLimit={e.timeLimitSeconds}");
        }
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

        Debug.Log($"MiniGameSwapper: Loading scene '{entry.sceneName}' at entry index {index} using mode {(useSingle ? LoadSceneMode.Single : LoadSceneMode.Additive)}.");
        Debug.Log($"MiniGameSwapper: currentIndex={currentIndex}, entriesCount={miniGameSceneEntries.Count}.");
        
        StopSceneTimer();
        var loadMode = useSingle ? LoadSceneMode.Single : LoadSceneMode.Additive;
        if (useSingle)
        {
            loadedSceneNames.Clear();
        }
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
        Debug.Log($"MiniGameSwapper: OnMiniGameComplete called at currentIndex {currentIndex}.");
        StopSceneTimer();

        if (currentIndex < 0 || currentIndex >= miniGameSceneEntries.Count)
        {
            Debug.LogWarning($"MiniGameSwapper: currentIndex {currentIndex} is out of range on complete.");
            return;
        }

        var completedEntry = miniGameSceneEntries[currentIndex];
        currentIndex++;

        if (currentIndex >= miniGameSceneEntries.Count)
        {
            Debug.Log("MiniGameSwapper: Last minigame completed. Resetting to first scene.");

            if (completedEntry != null && !completedEntry.stayLoaded)
            {
                UnloadSceneIfLoaded(completedEntry.sceneName);
            }

            currentIndex = 0;
            LoadSceneEntry(currentIndex, true);
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

        Debug.Log("MiniGameSwapper: CompleteCurrentMiniGame invoked.");

        instance.OnMiniGameComplete();
    }
}
