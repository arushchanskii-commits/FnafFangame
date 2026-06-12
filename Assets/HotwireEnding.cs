using UnityEngine;

public class HotwireEnding : MonoBehaviour
{
    [Tooltip("Tag of the object that should trigger the next minigame when it touches this object.")]
    public string targetTag = "Target";

    private void OnTriggerEnter(Collider other)
    {
        TryAdvanceMiniGame(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryAdvanceMiniGame(collision.gameObject);
    }

    private void TryAdvanceMiniGame(GameObject other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        MiniGameSwapper.CompleteCurrentMiniGame();
    }
}
