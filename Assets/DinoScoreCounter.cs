using UnityEngine;
using UnityEngine.SceneManagement;

public class DinoScoreCounter : MonoBehaviour
{
    [Tooltip("Prefab used by cactus objects to score points.")]
    public GameObject cactusPrefab;

    [Tooltip("Prefab used by bird objects to score points.")]
    public GameObject birdPrefab;

    [Tooltip("Score required to advance to the next scene.")]
    public int scoreThreshold = 5;

    [Tooltip("Optional UI Text component to display the current score.")]
    public UnityEngine.UI.Text scoreText;

    private int currentScore;
    private bool levelAdvanced;

    private void Start()
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    private void AddPoint()
    {
        if (levelAdvanced)
            return;

        currentScore++;
        UpdateScoreText();

        if (currentScore >= scoreThreshold)
        {
            AdvanceToNextScene();
        }
    }

    private bool IsScoringObject(GameObject other)
    {
        return PrefabMatches(other, cactusPrefab) || PrefabMatches(other, birdPrefab);
    }

    private static bool PrefabMatches(GameObject other, GameObject prefab)
    {
        if (prefab == null || other == null)
            return false;

        string otherName = CleanName(other.name);
        string prefabName = CleanName(prefab.name);
        return otherName == prefabName;
    }

    private static string CleanName(string rawName)
    {
        return rawName.Replace("(Clone)", "").Trim();
    }

    private void AdvanceToNextScene()
    {
        levelAdvanced = true;

        MiniGameSwapper swapper = FindObjectOfType<MiniGameSwapper>();
        if (swapper != null)
        {
            MiniGameSwapper.CompleteCurrentMiniGame();
            return;
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("DinoScoreCounter: Score threshold reached, but there is no next scene in build settings.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsScoringObject(other.gameObject))
        {
            AddPoint();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsScoringObject(collision.gameObject))
        {
            AddPoint();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsScoringObject(other.gameObject))
        {
            AddPoint();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsScoringObject(collision.gameObject))
        {
            AddPoint();
        }
    }
}
