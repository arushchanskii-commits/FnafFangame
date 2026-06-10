using UnityEngine;
using UnityEngine.SceneManagement;

public class DinoScoreCounter : MonoBehaviour
{
    [Tooltip("Scene object that counts as the scoring target.")]
    public GameObject targetObject;

    [Tooltip("Optional tag to identify valid targets if the target object is not set.")]
    public string targetTag;

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
        if (other == null)
            return false;

        if (targetObject != null && other == targetObject)
            return true;

        if (!string.IsNullOrEmpty(targetTag) && other.CompareTag(targetTag))
            return true;

        return false;
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
